//
//  GMCameraAvSupport.m
//  GammaLiveDetect
//
//  Created by Chris on 2018/10/19.
//  Copyright © 2018 Chris. All rights reserved.
//

#import "GMCameraAvSupport.h"

@implementation GMCameraAvSupport

#pragma mark - AV Related

- (void)avInit {
    
}

- (void)avStartSession {
    [self.avSession startRunning];
}

- (void)avStopSession {
    [self.avSession stopRunning];
}

- (void)avCapture {
    
}

#pragma mark - Lazy Getter

- (AVCaptureVideoPreviewLayer *)previewlayer {
    if (nil == _previewlayer) {
        _previewlayer = [[AVCaptureVideoPreviewLayer alloc] initWithSession:self.avSession];
        _previewlayer.videoGravity = AVLayerVideoGravityResizeAspectFill;
    }
    return _previewlayer;
}

- (AVCaptureSession *)avSession {
    if (nil == _avSession) {
        _avSession = [[AVCaptureSession alloc] init];
        
        /* 分辨率设置 */
        if ([_avSession canSetSessionPreset:AVCaptureSessionPresetHigh]) {
            _avSession.sessionPreset = AVCaptureSessionPresetHigh;
        }
        
        /* 添加输入 */
        if ([_avSession canAddInput:self.deviceInput]) {
            [_avSession addInput:self.deviceInput];
        }
        
        /* 添加输出 */
        if ([_avSession canAddOutput:self.dataOutput]) {
            [_avSession addOutput:self.dataOutput];
        }
    }
    
    return _avSession;
}

- (AVCaptureDeviceInput *)deviceInput {
    if (nil == _deviceInput) {
        
        AVCaptureDevice *tmpDevice = [self frontCamera];
        
        /* 设置捕捉频率 */
        float frameRate = 2;
        for(AVCaptureDeviceFormat *vFormat in [tmpDevice formats] ) {
            CMFormatDescriptionRef description = vFormat.formatDescription;
            float maxRate = ((AVFrameRateRange*)[vFormat.videoSupportedFrameRateRanges objectAtIndex:0]).maxFrameRate;
            if (maxRate > frameRate - 1
                && CMFormatDescriptionGetMediaSubType(description) == kCVPixelFormatType_420YpCbCr8BiPlanarFullRange) {
                
                if ([tmpDevice lockForConfiguration:nil]) {
                    tmpDevice.activeFormat = vFormat;
                    [tmpDevice setActiveVideoMinFrameDuration:CMTimeMake(10, frameRate * 10)];
                    [tmpDevice setActiveVideoMaxFrameDuration:CMTimeMake(10, frameRate * 10)];
                    [tmpDevice unlockForConfiguration];
                    break;
                }
            }
        }
        
        /* 设置曝光平衡 */
        if ([tmpDevice lockForConfiguration:nil]) {
            
            /* 闪光灯关闭 */
            if ([tmpDevice isFlashModeSupported:AVCaptureFlashModeOff]) {
                [tmpDevice setFlashMode:AVCaptureFlashModeOff];
            }
            
            /* 自动白平衡 */
            if ([tmpDevice isWhiteBalanceModeSupported:AVCaptureWhiteBalanceModeAutoWhiteBalance]) {
                [tmpDevice setWhiteBalanceMode:AVCaptureWhiteBalanceModeAutoWhiteBalance];
            }
            
            [tmpDevice unlockForConfiguration];
        }
        
        /* 自动聚焦 & 自动曝光 */
        if ([tmpDevice lockForConfiguration:nil]) {
            
            CGPoint centerPoint = CGPointMake(0.5f, 0.5f);
            
            /* 自动聚焦 */
            AVCaptureFocusMode focusMode = AVCaptureFocusModeContinuousAutoFocus;
            BOOL canResetFocus =
            [tmpDevice isFocusPointOfInterestSupported]
            && [tmpDevice isFocusModeSupported:focusMode];
            
            if (canResetFocus) {
                tmpDevice.focusMode = focusMode;
                tmpDevice.focusPointOfInterest = centerPoint;
            }
            
            /* 曝光 */
            AVCaptureExposureMode exposureMode = AVCaptureExposureModeContinuousAutoExposure;
            BOOL canResetExposure =
            [tmpDevice isExposurePointOfInterestSupported]
            && [tmpDevice isExposureModeSupported:exposureMode];
            
            if (canResetExposure) {
                tmpDevice.exposureMode = exposureMode;
                tmpDevice.exposurePointOfInterest = centerPoint;
            }
            
            [tmpDevice unlockForConfiguration];
        }
        
        /* 输入设备 */
        NSError *error = nil;
        _deviceInput = [[AVCaptureDeviceInput alloc] initWithDevice:tmpDevice error:&error];
    }
    
    return _deviceInput;
}

- (AVCaptureVideoDataOutput *)dataOutput {
    if (nil == _dataOutput) {
        _dataOutput = [[AVCaptureVideoDataOutput alloc] init];
        _dataOutput.alwaysDiscardsLateVideoFrames = YES;
        [_dataOutput setSampleBufferDelegate:_owener queue:self.videoQueue];
        
        NSDictionary *videoSettingDic =
        @{
          (id)kCVPixelBufferPixelFormatTypeKey:@(kCVPixelFormatType_32BGRA)
          };
        [_dataOutput setVideoSettings:videoSettingDic];
    }
    
    return _dataOutput;
}

- (dispatch_queue_t)videoQueue {
    if (nil == _videoQueue) {
        _videoQueue = dispatch_queue_create("com.cmcamera.videoqueue", DISPATCH_QUEUE_SERIAL);
    }
    return _videoQueue;
}

#pragma mark - AV Camera Position

- (AVCaptureDevice *)frontCamera {
    NSArray *cameras = [AVCaptureDevice devicesWithMediaType:AVMediaTypeVideo];
    for (AVCaptureDevice *camera in cameras) {
        if ([camera position] == AVCaptureDevicePositionFront) {
            return camera;
        }
    }
    return nil;
}

- (AVCaptureDevice *)backCamera {
    NSArray *cameras = [AVCaptureDevice devicesWithMediaType:AVMediaTypeVideo];
    for (AVCaptureDevice *camera in cameras) {
        if ([camera position] == AVCaptureDevicePositionBack) {
            return camera;
        }
    }
    return nil;
}

#pragma mark - AV Runtime Config

- (void)setAvFlashOn:(BOOL)avFlashOn {
    
    if (_avFlashOn == avFlashOn) {
        return;
    }
    
    AVCaptureDevice *tmpDevice = [self backCamera];
    
    if (YES == avFlashOn) {
        
        if (NO == [tmpDevice hasTorch]) {
            return;
        }
        
        if (NO == [tmpDevice lockForConfiguration:nil]) {
            return;
        }
        
        [tmpDevice setTorchMode:AVCaptureTorchModeOn];
        [tmpDevice unlockForConfiguration];
        
    } else {
        
        if (NO == [tmpDevice lockForConfiguration:nil]) {
            return;
        }
        
        [tmpDevice setTorchMode:AVCaptureTorchModeOff];
        [tmpDevice unlockForConfiguration];
    }
    
    _avFlashOn = avFlashOn;
}

- (void)setAvBackCamera:(BOOL)avBackCamera {
    
    if (_avBackCamera == avBackCamera) {
        return;
    }
    
    [self.avSession stopRunning];
    
    /* 当前镜头 */
    AVCaptureDevicePosition curPostion = self.deviceInput.device.position;
    
    /* 选择新摄像头 */
    AVCaptureDevice *tmpDevice = nil;
    if (AVCaptureDevicePositionFront == curPostion) {
        tmpDevice = [self backCamera];
    } else {
        tmpDevice = [self frontCamera];
    }
    
    /* 构造新设备输入 */
    AVCaptureDeviceInput *newDeviceInput = [AVCaptureDeviceInput deviceInputWithDevice:tmpDevice error:nil];
    
    /* 替换设备输入 */
    [self.avSession beginConfiguration];
    [self.avSession removeInput:self.deviceInput];
    [self.avSession addInput:newDeviceInput];
    [self.avSession commitConfiguration];
    self.deviceInput = newDeviceInput;
    
    [self.avSession startRunning];
    [self.previewlayer addAnimation:[self cameraChangeAnimation] forKey:nil];
    
    _avBackCamera = avBackCamera;
}

#pragma mark - 数据转化

+ (UIImage *)uiimageFromSampleBuffer:(CMSampleBufferRef)bufferRef fromBackCamera:(BOOL)fromBackCamera {
    
    CVImageBufferRef imageBuffer = CMSampleBufferGetImageBuffer(bufferRef);
    
    CGImageRef quartzImage = nil;
    CVPixelBufferLockBaseAddress(imageBuffer, 0);
    {
        void *baseAddress = CVPixelBufferGetBaseAddress(imageBuffer);
        
        size_t bytesPerRow = CVPixelBufferGetBytesPerRow(imageBuffer);
        size_t width = CVPixelBufferGetWidth(imageBuffer);
        size_t height = CVPixelBufferGetHeight(imageBuffer);
        
        CGColorSpaceRef colorSpace = CGColorSpaceCreateDeviceRGB();
        
        CGContextRef context =
        CGBitmapContextCreate(baseAddress,
                              width,
                              height,
                              8,
                              bytesPerRow,
                              colorSpace,
                              kCGBitmapByteOrder32Little | kCGImageAlphaPremultipliedFirst
                              );
        quartzImage = CGBitmapContextCreateImage(context);
        
        CGContextRelease(context);
        CGColorSpaceRelease(colorSpace);
    }
    CVPixelBufferUnlockBaseAddress(imageBuffer,0);
    
    /* 前摄像头取 LeftMirrored， 后摄像头取Right */
    UIImage *tmpImage = nil;
    if (fromBackCamera) {
        tmpImage = [UIImage imageWithCGImage:quartzImage scale:1 orientation:UIImageOrientationRight];
    } else {
        tmpImage = [UIImage imageWithCGImage:quartzImage scale:1 orientation:UIImageOrientationLeftMirrored];
    }
    
    CGImageRelease(quartzImage);
    
    return tmpImage;
}

#pragma mark - MISC

- (CAAnimation *)cameraChangeAnimation {
    CATransition *animation = [CATransition animation];
    animation.timingFunction = [CAMediaTimingFunction functionWithName:kCAMediaTimingFunctionEaseInEaseOut];
    animation.duration = 0.25;
    animation.type = @"oglFlip";
    animation.subtype = kCATransitionFromLeft;
    return animation;
}

@end
