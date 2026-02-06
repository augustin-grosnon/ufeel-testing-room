#pragma once

#include "opencv2/opencv.hpp"
#include "opencv2/objdetect.hpp"
#include <string>
#include <vector>
#include <map>

class EmotionDetector
{
    public:
        EmotionDetector(const std::string &modelPath, const std::string &yuNetModelPath);
        ~EmotionDetector() = default;

        std::vector<float> predict(const cv::Mat &face);
        std::vector<std::map<std::string, float>> predictFromImage(const cv::Mat &image);

    private:
        cv::dnn::Net _net;
        cv::Mat preprocess(const cv::Mat &face);
        cv::Ptr<cv::FaceDetectorYN> _faceDetector;
        std::vector<std::string> _classLabels = {"happiness","surprise","sadness","anger","neutral","fear"};
};
