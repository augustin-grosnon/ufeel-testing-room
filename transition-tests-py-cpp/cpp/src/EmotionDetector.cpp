#include "EmotionDetector.hpp"
#include <algorithm>
#include <cmath>
#include <stdexcept>

EmotionDetector::EmotionDetector(const std::string &modelPath, const std::string &yuNetModelPath)
{
    _net = cv::dnn::readNetFromONNX(modelPath);
    _faceDetector = cv::FaceDetectorYN::create(yuNetModelPath, "", cv::Size(320, 320), 0.9);
}

cv::Mat EmotionDetector::preprocess(const cv::Mat &face)
{
    cv::Mat resized, rgb, gray;
    cv::resize(face, resized, cv::Size(64,64));
    cv::cvtColor(resized, rgb, cv::COLOR_BGR2RGB);
    cv::cvtColor(rgb, gray, cv::COLOR_RGB2GRAY);
    cv::Mat gray3;
    cv::merge(std::vector<cv::Mat>{gray, gray, gray}, gray3);
    gray3.convertTo(gray3, CV_32FC3, 1.0 / 255.0);
    std::vector<cv::Mat> ch(3);
    cv::split(gray3, ch);
    const float mean[3] { 0.485f, 0.456f, 0.406f };
    const float std[3] { 0.229f, 0.224f, 0.225f };
    for (int i = 0; i < 3; i++)
        ch[i] = (ch[i] - mean[i]) / std[i];
    cv::merge(ch, gray3);
    return gray3;
}

std::vector<float> EmotionDetector::predict(const cv::Mat &face)
{
    cv::Mat input = preprocess(face);
    std::vector<float> tensorValues(3 * 64 * 64);
    int idx = 0;
    for (int c = 0; c < 3; c++)
        for (int y = 0; y < 64; y++)
            for (int x = 0; x < 64; x++)
                tensorValues[idx++] = input.at<cv::Vec3f>(y, x)[c];

    cv::Mat blob = cv::dnn::blobFromImage(input);
    _net.setInput(blob);
    cv::Mat output = _net.forward();

    std::vector<float> scores(output.ptr<float>(), output.ptr<float>() + output.total());

    float maxLogit = *std::max_element(scores.begin(), scores.end());
    float sum = 0.0f;
    for (auto &s : scores) {
        s = std::exp(s - maxLogit);
        sum += s;
    }
    for (auto &s : scores)
        s /= sum;

    float neutralFactor = 0.001f;
    scores[4] *= neutralFactor;

    float total = 0.0f;
    for (auto s : scores)
        total += s;
    for (auto &s : scores)
        s /= total;

    return scores;
}

std::vector<std::map<std::string,float>> EmotionDetector::predictFromImage(const cv::Mat &image)
{
    cv::Mat resized;
    cv::Mat inputImage = image.clone();
    const cv::Size yunetSize(320,320);
    cv::resize(inputImage, resized, yunetSize);
    _faceDetector->setInputSize(yunetSize);

    cv::Mat faces;
    _faceDetector->detect(resized, faces);
    if (faces.empty()) return {};

    std::vector<std::map<std::string, float>> allScores;
    float scaleX = float(image.cols) / yunetSize.width;
    float scaleY = float(image.rows) / yunetSize.height;

    for (int i = 0; i < faces.rows; i++) {
        float conf = faces.at<float>(i, 14);
        if (conf < 0.9f) continue;
        cv::Rect faceRect(
            int(faces.at<float>(i, 0) * scaleX),
            int(faces.at<float>(i, 1) * scaleY),
            int(faces.at<float>(i, 2) * scaleX),
            int(faces.at<float>(i, 3) * scaleY)
        );
        faceRect &= cv::Rect(0, 0, image.cols, image.rows);
        std::vector<float> scores = predict(inputImage(faceRect));
        std::map<std::string, float> labeled;
        for(size_t j = 0; j < _classLabels.size(); j++)
            labeled[_classLabels[j]] = scores[j];
        allScores.push_back(labeled);
    }
    return allScores;
}
