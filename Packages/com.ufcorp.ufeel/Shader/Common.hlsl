#ifndef _EMOTIONDETECTIONBARRACUDA_COMMON_H_
#define _EMOTIONDETECTIONBARRACUDA_COMMON_H_

#include "Struct.hlsl"

#define MAX_DETECTION 8

struct Detection
{
    uint classIndex;
    float score;
};

float Sigmoid(float x)
{
    return 1 / (1 + exp(-x));
}


#endif
