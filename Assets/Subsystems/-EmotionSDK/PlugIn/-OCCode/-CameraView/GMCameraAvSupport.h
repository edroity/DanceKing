//
//  GMCameraAvSupport.h
//  GammaLiveDetect
//
//  Created by Chris on 2018/10/19.
//  Copyright © 2018 Chris. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface GMCameraAvSupport : NSObject

@property (nonatomic, weak) id<AVCaptureVideoDataOutputSampleBufferDelegate> owener;

/* AVSession Related */
@property (nonatomic, strong) AVCaptureVideoPreviewLayer *previewlayer;     /* 预览图层，依赖avSession */
@property (nonatomic, strong) AVCaptureSession *avSession;                  /* 流对象，依赖deviceInput, deviceOutput */
@property (nonatomic, strong) AVCaptureDeviceInput *deviceInput;            /* 设备输入 */
@property (nonatomic, strong) AVCaptureVideoDataOutput *dataOutput;         /* 数据输出，依赖videoQueue */
@property (nonatomic, strong) dispatch_queue_t videoQueue;

- (void)avInit;             /* 初始化 */
- (void)avStartSession;     /* 开始视频流 */
- (void)avStopSession;      /* 暂停视频流 */
- (void)avCapture;          /* 抓取当前图片信息 */

@property (nonatomic, assign) BOOL avFlashOn;           /* 闪光灯是否开启，默认 NO */
@property (nonatomic, assign) BOOL avBackCamera;        /* 是否使用后置摄像头，默认 NO */

#pragma mark - 数据转化

+ (UIImage *)uiimageFromSampleBuffer:(CMSampleBufferRef)bufferRef fromBackCamera:(BOOL)fromBackCamera;

@end

NS_ASSUME_NONNULL_END

