//
//  NSObject_a.h
//  Unity-iPhone
//
//  Created by zhiyuan.peng on 2017/10/9.
//
//

#import <Foundation/Foundation.h>
#import "TestBridgeClass.h"
#import "Gate.h"
#import <YZTEmotionSdk/YZTEmotionManager.h>
#import "NativeEmotion.h"
#import <JSONMatcher/JSONMatcher.h>
#import "AVDetectionManager.h"

//USING_NS_CC;
static AVDetectionManager *detectionManager = nil;
@implementation NativeEmotion : NSObject

+ (UIImage*) Base64ToImage:(NSString*) base64String
{
    if(base64String == nil)
    {
        NSLog(@"[Base64ToImage] base64String is nil");
        return nil;
    }
    NSData *decodedData = [[NSData alloc] initWithBase64EncodedString:base64String options:NSDataBase64DecodingIgnoreUnknownCharacters];

    if(decodedData == nil)
    {
        NSLog(@"[Base64ToImage] data is nil");
        return nil;
    }
    
    UIImage *image = [UIImage imageWithData:decodedData];
    if(image == nil)
    {
        NSLog(@"[Base64ToImage] image is nil");
        return nil;
    }
    return image;
}

+ (NSDictionary*) FaceItemToDictionaryCulled:(YZTFaceItem*) face
{
    id dic = [[NSMutableDictionary alloc] init];
    id leftEyesPos = [NativeEmotion CGPointToDictionary:face.leftEyesPos];
    id leftMouthCornerPos = [NativeEmotion CGPointToDictionary:face.leftMouthCornerPos];
    id rightMouthCornerPos = [NativeEmotion CGPointToDictionary:face.rightMouthCornerPos];
    
    [dic setObject:leftEyesPos forKey:@"leftEyesPos"];
    
    [dic setObject:leftMouthCornerPos forKey:@"leftMouthCornerPos"];
    [dic setObject:rightMouthCornerPos forKey:@"rightMouthCornerPos"];
    
    return dic;
}

+ (NSDictionary*) FaceItemToDictionary:(YZTFaceItem*) face
{
    id dic = [[NSMutableDictionary alloc] init];
    id faceRect = [NativeEmotion CGRectToDictionary:face.faceRect];
    id isMaxFace = face.isMaxFace ? @1 : @0;
    id leftEyesPos = [NativeEmotion CGPointToDictionary:face.leftEyesPos];
    id rightEyesPos = [NativeEmotion CGPointToDictionary:face.rightEyesPos];
    id leftMouthCornerPos = [NativeEmotion CGPointToDictionary:face.leftMouthCornerPos];
    id rightMouthCornerPos = [NativeEmotion CGPointToDictionary:face.rightMouthCornerPos];
    id emotion = face.emotion;
    id direction = face.direction;
    
    [dic setObject:faceRect forKey:@"faceRect"];
    [dic setObject:isMaxFace forKey:@"isMaxFace"];
    [dic setObject:leftEyesPos forKey:@"leftEyesPos"];
    [dic setObject:rightEyesPos forKey:@"rightEyesPos"];
    
    [dic setObject:leftMouthCornerPos forKey:@"leftMouthCornerPos"];
    [dic setObject:rightMouthCornerPos forKey:@"rightMouthCornerPos"];
    if(emotion != nil)
    {
        [dic setObject:emotion forKey:@"emotion"];
    }
    if(direction != nil)
    {
        [dic setObject:direction forKey:@"direction"];
    }

    return dic;
}

+ (NSDictionary*) CGRectToDictionary:(CGRect) rect
{
    id dic = [[NSMutableDictionary alloc] init];
    id origin = [NativeEmotion CGPointToDictionary:rect.origin];
    id size = [NativeEmotion CGSizeToDictionary:rect.size];
    [dic setObject:origin forKey:@"origin"];
    [dic setObject:size forKey:@"size"];
    return dic;
}

+ (NSDictionary*) CGSizeToDictionary:(CGSize) size
{
    id dic = [[NSMutableDictionary alloc] init];
    id width = [NSNumber numberWithDouble:size.width];
    id height = [NSNumber numberWithDouble:size.height];
    [dic setObject:width forKey:@"width"];
    [dic setObject:height forKey:@"height"];
    return dic;
}

+ (NSDictionary*) CGPointToDictionary:(CGPoint) point
{
    id dic = [[NSMutableDictionary alloc] init];
    id x = [NSNumber numberWithDouble:point.x];
    id y = [NSNumber numberWithDouble:point.y];
    [dic setObject:x forKey:@"x"];
    [dic setObject:y forKey:@"y"];
    return dic;
}

+ (NSArray<NSDictionary*>*) FaceListToDictionaryList:(NSArray<YZTFaceItem *>*) list
{
    id ret = [[NSMutableArray alloc] init];
    for(int i = 0; i < list.count; i++)
    {
        id face = [list objectAtIndex:i];
        id faceDic = [NativeEmotion FaceItemToDictionary:face];
        [ret addObject:faceDic];
    }
    return ret;
}

+ (void)SetEmotionCheck:(NSString*)callId arg:(NSString*)arg
{
    bool b = [arg isEqualToString:@"true"];
    [YZTEmotionManager shareInstance].hasEmotionCheck = b;
}

+ (void)SetDirectionCheck:(NSString*)callId arg:(NSString*)arg
{
    bool b = [arg isEqualToString:@"true"];
    [YZTEmotionManager shareInstance].hasDirectionCheck = b;
}

+ (void)InitDetection:(NSString*)callId arg:(NSString*)arg
{
    if(detectionManager) return;
     NSLog(@"[InitDetection] init suc1");
    detectionManager = [AVDetectionManager new];
    //    [detectionManager loadEmotionMatchData];
      NSLog(@"[InitDetection] init suc2");
    detectionManager.detectionBlock = ^(NSDictionary<NSString *, NSNumber*> *matchDisDic, NSArray<NSNumber *>* headArr) {
        NSLog(@"[InitDetection] detectionManager.detectionBlock");
        //        int gameTextAnimationIndex = -1;
        //        if (matchDisDic == nil || matchDisDic.count == 0 || headArr.count == 0) {
        //            gameTextAnimationIndex = 4;
        //            doubleHitCount_ = 0;
        //            return;
        //        }
        //
        //        bool isMatchEmotion = false;
        //
        //        cocos2d::Vector<Emotion*> emotionsBackup;
        //        if (state == GameStateCrzay) {
        //            emotionsBackup = crzayEmotions_;
        //        } else {
        //            emotionsBackup = visibleEmotions_;
        //        }
        //
        //        if (emotionsBackup.empty() || emotionsBackup.size() == 0) {
        //            return;
        //        }
        //
        //        for (ssize_t i = emotionsBackup.size() - 1; i >= 0; i--) {
        //            if (emotionsBackup.empty() || emotionsBackup.size() == 0) {
        //                break;
        //            }
        //            auto em = emotionsBackup.at(i);
        //            auto position = em->getPosition();
        //            if (state == GameStateCrzay || (position.y > 130 && position.y < 380)) {
        //                int animationIndicator = em->animationIndicator;
        //
        //                int emtionType = animationIndicator / 10;
        //                NSString *emtionTypeName = @"";
        //
        //                if (emtionType == 1) {
        //                    emtionTypeName = @"minzui";
        //                } else if (emtionType == 2) {
        //                    emtionTypeName = @"zhoumei";
        //                } else if (emtionType == 3) {
        //                    emtionTypeName = @"zhangzuixiao";
        //                } else if (emtionType == 4) {
        //                    emtionTypeName = @"bizuixiao";
        //                } else if (emtionType == 5) {
        //                    emtionTypeName = @"biyanyou";
        //                } else if (emtionType == 6) {
        //                    emtionTypeName = @"biyanzuo";
        //                } else if (emtionType == 7) {
        //                    emtionTypeName = @"tushe";
        //                } else {
        //                    continue;
        //                }
        //
        //                NSNumber *distanceNum = [matchDisDic objectForKey:emtionTypeName];
        //                float distance = distanceNum.floatValue;
        //                int score = 0;
        //
        //                bool matchEmotionFlag = isEmationMatchAu(distance, emtionType, score);
        //
        //                if (!matchEmotionFlag) {
        //                    continue;
        //                }
        //
        //                int headType = animationIndicator % 10;
        //                // 摇头方向，点头方向，侧头方向
        //                bool matchHeadFlag = isEmationMatchHeadOrientation(headArr[1].floatValue, headArr[0].floatValue, headArr[2].floatValue, headType);
        //
        //                if (!matchHeadFlag) {
        //                    continue;
        //                }
        //
        //                if (matchEmotionFlag && matchHeadFlag) {
        //                    isMatchEmotion = true;
        //                    doubleHitCount_++;
        //                    if (state != GameStateCrzay) {
        //                        em->stopAllActions();
        //                        em->setScore(scoreSprites_.at(score));
        //                        em->setState(EmotionStateDone);
        //                        visibleEmotions_.eraseObject(em);
        //                        emotionSprites_.pushBack(em);
        //                    } else {
        //                        showScoreAnimation(score);
        //                    }
        //                    addScore((score + 1) * 5);
        //                }
        //            }
        //        }
        //
        //        if (isMatchEmotion) {
        //            if (doubleHitCount_ == 1) {
        //                gameTextAnimationIndex = 0;
        //            } else if (doubleHitCount_ == 5) {
        //                gameTextAnimationIndex = 1;
        //            } else if (doubleHitCount_ == 10) {
        //                gameTextAnimationIndex = 2;
        //            } else if (doubleHitCount_ >= 15) {
        //                gameTextAnimationIndex = 3;
        //                // reset double hit
        //                doubleHitCount_ = 0;
        //            }
        //            // TODO: play top text animation
        //            showSpecialLayer(gameTextAnimationIndex);
        //        } else {
        //            doubleHitCount_ = 0;
        //        }
    };

//        auto director = Director::getInstance();
//        auto glview = director->getOpenGLView();
//    CCEAGLView *eaglview = (CCEAGLView *)glview->getEAGLView();

        auto glview = UnityGetGLView();
        glview.backgroundColor = [UIColor clearColor];
        UIView *videoView = [glview.superview viewWithTag:1];
        [videoView addSubview:detectionManager.previewView];
    
    [detectionManager cameraRun];
}

+ (void)OpenDetection:(NSString*)callId arg:(NSString*)arg {
     NSLog(@"[OpenDetection]");
    [detectionManager cameraRun];
}

+ (void)CloseDetection:(NSString*)callId arg:(NSString*)arg {
    NSLog(@"[CloseDetection]");
    [detectionManager cameraClose];
}

// arg: base64ImageData
+ (void)Check:(NSString*)callId arg:(NSString*)arg
{
   // NSLog(@"[OC] NativeEmotion.Check");
    //NSLog(@"[OC] data: %@", arg);
    UIImage* image = [NativeEmotion Base64ToImage:arg];
   // NSLog(@"[OC] image: %@", image);
    
    [[YZTEmotionManager shareInstance] startCheckWithImg:image faceInfo:^(NSArray<YZTFaceItem *> * _Nonnull faceItems, ResultType result) {
        
        // 如果 sdk 报告检测失败，或者图片中没有人脸
        if (result != ResultTypeOfSuccess || faceItems.count == 0)
        {
            gateCallReturn(callId, @"{\"list\":[]}");
            return;
        }
        
        // NSLog(@"startCheckWithImg --- faceItems : %@ --result : %d\n", faceItems, result);
        id dic = [[NSMutableDictionary alloc] init];
        id faceList = [NativeEmotion FaceListToDictionaryList:faceItems];
        [dic setObject:faceList forKey:@"list"];
        id json = [dic toJSONString];
        
        //NSLog(@"dic: %@", dic);
        //NSLog(@"%@", json);
        gateCallReturn(callId, json);
    }];
 
}

// arg: base64ImageData
+ (void)CheckCulled:(NSString*)callId arg:(NSString*)arg
{
    // NSLog(@"[OC] NativeEmotion.Check");
    //NSLog(@"[OC] data: %@", arg);
    UIImage* image = [NativeEmotion Base64ToImage:arg];
    // NSLog(@"[OC] image: %@", image);
    
    [[YZTEmotionManager shareInstance] startCheckWithImg:image faceInfo:^(NSArray<YZTFaceItem *> * _Nonnull faceItems, ResultType result) {
        
        // 如果 sdk 报告检测失败，或者图片中没有人脸
        if (result != ResultTypeOfSuccess || faceItems.count == 0)
        {
            gateCallReturn(callId, @"");
            return;
        }
        
        // NSLog(@"startCheckWithImg --- faceItems : %@ --result : %d\n", faceItems, result);
        id firstFace = [faceItems objectAtIndex:0];
        
        id faceDic = [NativeEmotion FaceItemToDictionaryCulled:firstFace];
        id json = [faceDic toJSONString];
        
        //NSLog(@"dic: %@", dic);
        //NSLog(@"%@", json);
        gateCallReturn(callId, json);
    }];
    
}

@end

