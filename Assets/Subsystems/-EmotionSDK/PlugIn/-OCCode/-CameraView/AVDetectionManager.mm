//
//  AVDetectionManager.m
//  FaceKing-mobile
//
//  Created by 陈乐辉 on 2018/12/31.
//

#import "AVDetectionManager.h"
#import <GammaEcosystem/GammaEcosystem.h>
#import <GammaEcosystem/GMToolkitOc.h>
#import "GMCameraAvSupport.h"

@interface AVDetectionManager () <AVCaptureVideoDataOutputSampleBufferDelegate>

@property (nonatomic, strong) GMFaceDetector *faceDetector;
@property (nonatomic, strong) GMAuDetector *auDetector;
@property (nonatomic, strong) GMHeadOrientationDetector *headOrientationDetector;
@property (nonatomic, strong) GMCameraAvSupport *avSupport;
@property (nonatomic, strong) NSDictionary *comparyEmotionDic;

@end

@implementation AVDetectionManager

- (instancetype)init {
    NSLog(@"==========AVDetectionManager init=============");
    if (self = [super init]) {
        _faceDetector = [[GMFaceDetector alloc] init];
        [_faceDetector loadModels];
        
        _headOrientationDetector = [[GMHeadOrientationDetector alloc] init];
        [_headOrientationDetector loadModels];
        
        _auDetector = [[GMAuDetector alloc] init];
        [_auDetector loadModels];
        
        _avSupport = [[GMCameraAvSupport alloc] init];
        _avSupport.owener = self;
        
        CGRect frame = [UIScreen mainScreen].bounds;
        _previewView = [[UIView alloc] initWithFrame:frame];
        [_previewView.layer addSublayer:_avSupport.previewlayer];
        [_avSupport.previewlayer setFrame:frame];
         NSLog(@"==========AVDetectionManager init suc=============");
    }
    
    return self;
}

- (void)loadEmotionMatchData {
    //NSURL *fileUrl = [[NSBundle mainBundle] URLForResource:@"au_feature" withExtension:@"csv"];
    NSURL *fileUrl = [[NSBundle mainBundle] URLForResource:@"au_128" withExtension:@"csv"];
    NSString *fileContents = [NSString stringWithContentsOfURL:fileUrl];
    NSArray *rows = [fileContents componentsSeparatedByString:@"\n"];
    NSMutableDictionary *dic = [NSMutableDictionary dictionary];
    for (NSString *row in rows){
        NSArray *columns = [row componentsSeparatedByString:@","];
        NSString *key = columns[0];
        
        if ([key length] > 0) {
            NSArray *values = [columns subarrayWithRange:NSMakeRange(1, columns.count - 1)];
            [dic setObject:values forKey:key];
        }
    }
    self.comparyEmotionDic = dic;
}

- (void)cameraRun {
    [self.avSupport avStartSession];
    self.isGamePlaying = true;
}

- (void)cameraClose {
    [self.avSupport avStopSession];
      self.isGamePlaying = false;
}

#pragma mark - AVCaptureVideoDataOutputSampleBufferDelegate

- (void)captureOutput:(AVCaptureOutput *)output didOutputSampleBuffer:(CMSampleBufferRef)sampleBuffer fromConnection:(AVCaptureConnection *)connection {
      NSLog(@"==========detect try=============s%",connection);
    if (connection != [self.avSupport.dataOutput connectionWithMediaType:AVMediaTypeVideo]) {
        return;
    }
    NSLog(@"==========detect connect=============");
    if (self.isGamePlaying){// && self.comparyEmotionDic && self.comparyEmotionDic.count) {
        NSLog(@"==========detect begin=============");
        UIImage *origImage = [GMCameraAvSupport uiimageFromSampleBuffer:sampleBuffer fromBackCamera:_avSupport.avBackCamera];
        UIImage *smallImage = [origImage gmResizeWithScale:(320 / origImage.size.width)];
        GMFaceItem *auFaceItem = [_faceDetector fetchMaxFaceInfo:smallImage withMode:GMFaceDetectModeAlignCut];
        NSArray *auArray = [_auDetector detect:auFaceItem.faceImage];
        
        GMFaceItem *headFaceItem = [_faceDetector fetchMaxFaceInfo:smallImage withMode:GMFaceDetectModeSingleCut];
        NSArray *headArray = [_headOrientationDetector detect:headFaceItem.faceImage];

        if (auArray.count || headArray.count) {
            if (self.detectionBlock) {
                NSMutableDictionary *matchDisDic = [NSMutableDictionary dictionary];
//                NSLog(@"auArray.count:%@,headArray.count:%@",auArray.count,headArray.count);
//                NSArray *keyArr = self.comparyEmotionDic.allKeys;
//                for(NSString *key : keyArr) {
//                    float cmpVal = [self.comparyEmotionDic[key] gmODistance:auArray];
//                    [matchDisDic setObject:[NSNumber numberWithFloat:cmpVal] forKey:key];
//                }

//                for (NSString *key : keyArr) {
//                   // NSLog(@"%@, %@", key, matchDisDic[key]);
//                    if ([key isEqualToString:@"zhangzuixiao"]) {
//                        NSLog(@"%@, %@", key, matchDisDic[key]);
//                    }
////                    if ([key isEqualToString:@"bizuixiao"]) {
////                        NSLog(@"%@, %@", key, matchDisDic[key]);
////                    }
////                    if ([key isEqualToString:@"biyanzuo"]) {
////                        NSLog(@"%@, %@", key, matchDisDic[key]);
////                    }
////                    if ([key isEqualToString:@"biyanyou"]) {
////                        NSLog(@"%@, %@", key, matchDisDic[key]);
////                    }
////                    if ([key isEqualToString:@"minzui"]) {
////                        NSLog(@"%@, %@", key, matchDisDic[key]);
////                    }
////                    if ([key isEqualToString:@"tushe"]) {
////                        NSLog(@"%@, %@", key, matchDisDic[key]);
////                    }
////                    if ([key isEqualToString:@"zhoumei"]) {
////                        NSLog(@"%@, %@", key, matchDisDic[key]);
////                    }
//                }
                NSLog(@"===========================");
                // 摇头方向，点头方向，侧头方向
                //NSLog(@"==%@==%@==%@", headArray[0], headArray[1], headArray[2]);
                NSLog(@"==%@==", headArray[2]);
                dispatch_async(dispatch_get_main_queue(), ^{
                    if (self.detectionBlock) {
                        self.detectionBlock(matchDisDic, headArray);
                    }
                });
            }
        }
    }
}

@end
