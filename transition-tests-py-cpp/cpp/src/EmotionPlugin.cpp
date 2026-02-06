#include "EmotionDetector.hpp"
#include <memory>

extern "C"
{
    static std::unique_ptr<EmotionDetector> globalDetector = nullptr; // TODO: avoid global variable if possible

    bool InitEmotionDetector(const char *emotionModelPath, const char *yuNetModelPath)
    {
        try {
            globalDetector = std::make_unique<EmotionDetector>(emotionModelPath, yuNetModelPath);
            return true;
        } catch (...) {
            return false;
        }
    }

    int DetectFromImage(unsigned char *data, int width, int height, int channels, float *outScores, int maxFaces)
    {
        if (!globalDetector) return -1;

        cv::Mat image(height, width, (channels == 3) ? CV_8UC3 : CV_8UC1, data);
        auto results = globalDetector->predictFromImage(image);

        int faceCount = std::min((int)results.size(), maxFaces);
        for (int i = 0; i < faceCount; i++) {
            int j = 0;
            for (auto &[_, score] : results[i])
                outScores[i * 6 + j++] = score;
        }

        return faceCount;
    }

    void DisposeEmotionDetector()
    {
        globalDetector.reset();
    }
}
