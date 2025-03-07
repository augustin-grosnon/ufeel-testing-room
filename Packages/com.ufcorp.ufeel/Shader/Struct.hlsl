#ifndef _EMOTIONDETECTIONBARRACUDA_STRUCT_H_
#define _EMOTIONDETECTIONBARRACUDA_STRUCT_H_

struct EmotionDetection
{
    float2 center;
    float2 extent;
    float2 keyPoints[6];
    float score;
    float3 pad;
};

#endif
