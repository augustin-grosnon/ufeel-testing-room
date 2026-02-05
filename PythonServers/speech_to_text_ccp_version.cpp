#include <vosk_api.h>
#include <portaudio.h>
#include <iostream>
#include <string>
#include <cstring>
#include <arpa/inet.h>
#include <unistd.h>

#define SAMPLE_RATE 16000
#define FRAMES_PER_BUFFER 4000

VoskRecognizer* recognizer;
int sockfd;
sockaddr_in servaddr;

static int audioCallback(
    const void* inputBuffer,
    void*,
    unsigned long framesPerBuffer,
    const PaStreamCallbackTimeInfo*,
    PaStreamCallbackFlags,
    void*
) {
    if (vosk_recognizer_accept_waveform(
            recognizer,
            (const char*)inputBuffer,
            framesPerBuffer * 2)) {

        const char* result = vosk_recognizer_result(recognizer);
        std::cout << "Result: " << result << std::endl;

        sendto(
            sockfd,
            result,
            strlen(result),
            0,
            (const sockaddr*)&servaddr,
            sizeof(servaddr)
        );
    }

    return paContinue;
}

int main() {
    vosk_set_log_level(0);

    // UDP
    sockfd = socket(AF_INET, SOCK_DGRAM, 0);
    servaddr.sin_family = AF_INET;
    servaddr.sin_port = htons(4244);
    inet_pton(AF_INET, "127.0.0.1", &servaddr.sin_addr);

    // Vosk
    VoskModel* model = vosk_model_new("models/vosk-model-small-fr-0.22");
    recognizer = vosk_recognizer_new(model, SAMPLE_RATE);

    // Audio
    Pa_Initialize();
    PaStream* stream;
    Pa_OpenDefaultStream(
        &stream,
        1,
        0,
        paInt16,
        SAMPLE_RATE,
        FRAMES_PER_BUFFER,
        audioCallback,
        nullptr
    );

    Pa_StartStream(stream);
    std::cout << "🎤 Listening..." << std::endl;

    while (true) {
        Pa_Sleep(100);
    }

    Pa_StopStream(stream);
    Pa_CloseStream(stream);
    Pa_Terminate();

    vosk_model_free(model);
    close(sockfd);
}



// to try 