//
//  IOSGameManifestManager.mm
//  Unity-iPhone
//
//  Created by zhiyuan.peng on 2017/5/18.
//
//

#import <Foundation/Foundation.h>
#import <objc/message.h>
#import "Util.h"
#import "NativeGameManifestManager.h"


@implementation NativeGameManifestManager : NSObject

NSString* manifestString;
NSDictionary* manifest;
bool loaded;

// Bridge method
// Sync call
+ (NSString*)GetRawString2:(NSString*)arg
{
    [NativeGameManifestManager tryLoad];
    if(manifestString == nil)
    {
        return @"";
    }
    else
    {
        return manifestString;
    }
    
}

+ (void) tryLoad
{
    if(!loaded)
    {
        loaded = true;
        [NativeGameManifestManager loadFromFile];
    }
    
}

// 从 conf.json 文件加载数据到变量中
+ (void) loadFromFile
{
    NSError *error;
    NSString *confPath=[NSString stringWithFormat:@"%@%@%@",[[NSBundle mainBundle]resourcePath],@"/",@"game-manifest.json"];
    NSString *jsonstring = [NSString stringWithContentsOfFile:confPath  encoding:NSUTF8StringEncoding error:nil];
    NSLog(@"[IOSGameManifestManager] manifest: %@",jsonstring);
    manifestString = jsonstring;
    NSData *data=[jsonstring dataUsingEncoding:NSUTF8StringEncoding];
    manifest = [NSJSONSerialization JSONObjectWithData:data options:kNilOptions error:&error];
}

+ (NSString*) tryGet: (NSString* )key default:(NSString* )defaultValue
{
    [NativeGameManifestManager tryLoad];
    id value = [manifest objectForKey:key];
    if(value == nil) return defaultValue;
    else return value;
}


@end
