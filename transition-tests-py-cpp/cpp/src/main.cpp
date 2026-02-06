#include "../include/EmotionDetector.hpp"
#include <iostream>

int main(int argc, char **argv)
{
    if (argc != 3) {
        std::cout << "Usage: " << argv[0] << " <emotion_model.onnx> <yunet_model.onnx>" << std::endl;
        return 1;
    }

    std::string emotionModelPath = argv[1];
    std::string yuNetModelPath   = argv[2];

    EmotionDetector detector(emotionModelPath, yuNetModelPath);

    cv::VideoCapture cap(0);
    if (!cap.isOpened()) {
        std::cerr << "Error: could not open camera" << std::endl;
        return 1;
    }

    cv::Mat frame;
    while (true) {
        cap >> frame;
        if (frame.empty()) break;

        auto results = detector.predictFromImage(frame);

        for (size_t i = 0; i < results.size(); i++) {
            const auto &scores = results[i];
            int baseY = 30 + 100 * int(i);
            int lineHeight = 25;

            int lineCount = 0;
            for (const auto &[label, score] : scores) {
                std::string text = label + ": " + std::to_string(int(score * 100)) + "%";
                cv::putText(frame, text, cv::Point(10, baseY + lineCount * lineHeight),
                            cv::FONT_HERSHEY_SIMPLEX, 0.7, cv::Scalar(0, 255, 0), 2);
                lineCount++;
            }
        }

        cv::imshow("Emotion Detection", frame);
        if (cv::waitKey(1) == 27) break;
    }

    cap.release();
    cv::destroyAllWindows();
    return 0;
}
