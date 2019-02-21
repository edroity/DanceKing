//
//  Tool.hpp
//  FaceKing-mobile
//
//  Created by 陈乐辉 on 2018/12/31.
//

#ifndef Tool_hpp
#define Tool_hpp

#include <stdio.h>


enum GameEmotionAnimationIndicator {
    
    GameEmotionAnimationIndicator_MinZuiSmileLeft = 10,
    GameEmotionAnimationIndicator_MinZuiSmileFront = 11,
    GameEmotionAnimationIndicator_MinZuiSmileRight = 12,
    GameEmotionAnimationIndicator_MinZuiSmileLeftWaiTou = 13,
    GameEmotionAnimationIndicator_MinZuiSmileRightWaiTou = 14,
    GameEmotionAnimationIndicator_MinZuiSmileUp = 15,
    GameEmotionAnimationIndicator_MinZuiSmileDown = 16,
    
    GameEmotionAnimationIndicator_LourLeft = 20,
    GameEmotionAnimationIndicator_LourFront = 21,
    GameEmotionAnimationIndicator_LourRight = 22,
    GameEmotionAnimationIndicator_LourLeftWaiTou = 23,
    GameEmotionAnimationIndicator_LourRightWaiTou = 24,
    GameEmotionAnimationIndicator_LourUp = 25,
    GameEmotionAnimationIndicator_LourDown = 26,
    
    GameEmotionAnimationIndicator_OpenMouseSmileLeft = 30,
    GameEmotionAnimationIndicator_OpenMouseSmileFront = 31,
    GameEmotionAnimationIndicator_OpenMouseSmileRight = 32,
    GameEmotionAnimationIndicator_OpenMouseSmileLeftWaiTou = 33,
    GameEmotionAnimationIndicator_OpenMouseSmileRightWaiTou = 34,
    GameEmotionAnimationIndicator_OpenMouseSmileUp = 35,
    GameEmotionAnimationIndicator_OpenMouseSmileDown = 36,

    GameEmotionAnimationIndicator_CloseMouseSmileLeft = 40,
    GameEmotionAnimationIndicator_CloseMouseSmileFront = 41,
    GameEmotionAnimationIndicator_CloseMouseSmileRight = 42,
    GameEmotionAnimationIndicator_CloseMouseSmileLeftWaiTou = 43,
    GameEmotionAnimationIndicator_CloseMouseSmileRightWaiTou = 44,
    GameEmotionAnimationIndicator_CloseMouseSmileUp = 45,
    GameEmotionAnimationIndicator_CloseMouseSmileDown = 46,

    GameEmotionAnimationIndicator_CloseRightEyeLeft = 50,
    GameEmotionAnimationIndicator_CloseRightEyeFront = 51,
    GameEmotionAnimationIndicator_CloseRightEyeRight = 52,
    GameEmotionAnimationIndicator_CloseRightEyeLeftWaiTou = 53,
    GameEmotionAnimationIndicator_CloseRightEyeRightWaiTou = 54,
    GameEmotionAnimationIndicator_CloseRightEyeUp = 55,
    GameEmotionAnimationIndicator_CloseRightEyeDown = 56,

    GameEmotionAnimationIndicator_CloseLeftEyeLeft = 60,
    GameEmotionAnimationIndicator_CloseLeftEyeFront = 61,
    GameEmotionAnimationIndicator_CloseLeftEyeRight = 62,
    GameEmotionAnimationIndicator_CloseLeftEyeLeftWaiTou = 63,
    GameEmotionAnimationIndicator_CloseLeftEyeRightWaiTou = 64,
    GameEmotionAnimationIndicator_CloseLeftEyeUp = 65,
    GameEmotionAnimationIndicator_CloseLeftEyeDown = 66,
    
    GameEmotionAnimationIndicator_TusheLeft = 70,
    GameEmotionAnimationIndicator_TusheFront = 71,
    GameEmotionAnimationIndicator_TusheRight = 72,
    GameEmotionAnimationIndicator_TusheLeftWaiTou = 73,
    GameEmotionAnimationIndicator_TusheRightWaiTou = 74,
    GameEmotionAnimationIndicator_TusheUp = 75,
    GameEmotionAnimationIndicator_TusheDown = 76,
};

enum GameMarketingEmotionAnimationIndicator {
    GameMarketingEmotionAnimationIndicator_CloseLeftEyeLeftWaiTou = 63,
    GameMarketingEmotionAnimationIndicator_CloseRightEyeRightWaiTou = 54,
    GameMarketingEmotionAnimationIndicator_LourLeftWaiTou = 23,
    GameMarketingEmotionAnimationIndicator_LourRightWaiTou = 24,
};

bool isEmationMatchAu(float auDistance, int emotionType, int& scoreIndex);

bool isEmationMatchHeadOrientation(float headUpDownDistance, float headLeftRightDistance, float headWaiTouDistance, int headType);

#endif /* Tool_hpp */
