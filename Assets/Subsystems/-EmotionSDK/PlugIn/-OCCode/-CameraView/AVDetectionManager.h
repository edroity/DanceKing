//
//  AVDetectionManager.h
//  FaceKing-mobile
//
//  Created by 陈乐辉 on 2018/12/31.
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>

typedef void (^AVDetectionManagerCallback)(NSDictionary<NSString *, NSNumber*> *, NSArray<NSNumber *> *);

NS_ASSUME_NONNULL_BEGIN

@interface AVDetectionManager : NSObject

@property (nonatomic, strong) UIView *previewView;
@property (nonatomic, copy, nullable) AVDetectionManagerCallback detectionBlock;
@property (nonatomic) bool isGamePlaying;

- (void)loadEmotionMatchData;
- (void)cameraRun;
- (void)cameraClose;

@end

NS_ASSUME_NONNULL_END
