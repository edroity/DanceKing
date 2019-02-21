//
//  Tool.cpp
//  FaceKing-mobile
//
//  Created by 陈乐辉 on 2018/12/31.
//

#include "Tool.h"

bool isEmationMatchAu(float auDistance, int emotionType, int& scoreIndex) {
    bool matchEmotionFlag = false;
    if (emotionType == 1) { // mizui
        if (auDistance >= 1.3) {
            return false;
        }
        matchEmotionFlag = true;
        
        if (auDistance < 1.3 && auDistance > 1.2) {
            scoreIndex = 0;
        }
        else if (auDistance <= 1.2 && auDistance > 1.1) {
            scoreIndex = 1;
        }
        else {
            scoreIndex = 3;
        }
    }
    else if (emotionType == 2) { // zhoumei
        if (auDistance >= 1.4) {
            return false;
        }
        matchEmotionFlag = true;
        
        if (auDistance < 1.4 && auDistance > 1.0) {
            scoreIndex = 0;
        }
        else if (auDistance <= 1.0 && auDistance > 0.8) {
            scoreIndex = 1;
        }
        else {
            scoreIndex = 3;
        }
    }
    else if (emotionType == 3) { // zhangzuixiao
        if (auDistance >= 1.4) {
            return false;
        }
        matchEmotionFlag = true;
        
        if (auDistance < 1.4 && auDistance > 1.2) {
            scoreIndex = 0;
        }
        else if (auDistance <= 1.2 && auDistance > 1) {
            scoreIndex = 1;
        }
        else {
            scoreIndex = 3;
        }
    }
    else if (emotionType == 4) { // bizuixiao
        if (auDistance >= 1.3) {
            return false;
        }
        matchEmotionFlag = true;
        
        if (auDistance < 1.3 && auDistance > 1.0) {
            scoreIndex = 0;
        }
        else if (auDistance <= 1.0 && auDistance > 0.8) {
            scoreIndex = 1;
        }
        else {
            scoreIndex = 3;
        }
    }
    else if (emotionType == 5) { // youbiyan
        if (auDistance >= 1.2) {
            return false;
        }
        matchEmotionFlag = true;
        
        if (auDistance < 1.2 && auDistance > 1.0) {
            scoreIndex = 0;
        }
        else if (auDistance <= 1.0 && auDistance > 0.9) {
            scoreIndex = 1;
        }
        else {
            scoreIndex = 3;
        }
    }
    else if (emotionType == 6) { // zuobiyan
        if (auDistance >= 1.2) {
            return false;
        }
        matchEmotionFlag = true;
        
        if (auDistance < 1.2 && auDistance > 1.0) {
            scoreIndex = 0;
        }
        else if (auDistance <= 1.0 && auDistance > 0.9) {
            scoreIndex = 1;
        }
        else {
            scoreIndex = 3;
        }
    }
    else if (emotionType == 7) { // tushe
        if (auDistance >= 1.5) {
            return false;
        }
        matchEmotionFlag = true;
        
        if (auDistance < 1.5 && auDistance > 1.2) {
            scoreIndex = 0;
        }
        else if (auDistance <= 1.2 && auDistance > 1.0) {
            scoreIndex = 1;
        }
        else {
            scoreIndex = 3;
        }
    }
    return matchEmotionFlag;
}

bool isEmationMatchHeadOrientation(float headUpDownDistance, float headLeftRightDistance, float headWaiTouDistance, int headType) {
    float head;
    bool matchHeadFlag = false;
    if (headType >= 0 && headType <= 2) {// left front right
        head = headLeftRightDistance;
        if (headType == 0 && !matchHeadFlag) { // left
            if (head > -1) {
                matchHeadFlag = true;
            }
        }
        else if (headType == 1 && !matchHeadFlag) {
            if (head > -20 || head < 20) {
                matchHeadFlag = true;
            }
        }
        else if (headType == 2 && !matchHeadFlag) {
            if (head < 1) {
                matchHeadFlag = true;
            }
        }
    }
    else if (headType == 3 || headType == 4) { // waitou
        head = headWaiTouDistance;
        if (headType == 3 && !matchHeadFlag) {
            if (head > -9) {
                matchHeadFlag = true;
            }
        }
        else if (headType == 4 && !matchHeadFlag) {
            if (head > 9) {
                matchHeadFlag = true;
            }
        }
        matchHeadFlag = true;
    }
    else if (headType == 5 || headType == 6) { // up down
        head = headUpDownDistance;
        if (headType == 5 && !matchHeadFlag) { // up
            if (head > 10) {
                matchHeadFlag = true;
            }
        }
        else if (headType == 6 && !matchHeadFlag) { // down
            if (head < -1) {
                matchHeadFlag = true;
            }
        }
    }
    return matchHeadFlag;
}
