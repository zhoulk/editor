#import "UnityAppController.h"
#import "MyUnityAppController.h"

#import <sys/utsname.h>
#import "iOSCommSdk/Constant.h"
#include "WXApiManager.h"
#import "LocationManager.h"
#include "SugramApiManager.h"
#include "MLIAPManager.h"

IMPL_APP_CONTROLLER_SUBCLASS(MyUnityAppController)

@implementation MyUnityAppController

- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    [super application:application didFinishLaunchingWithOptions:launchOptions];
    //[[Constant sharedInstance] readKeyValueForConstantFromPlist];
    wxAppId = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"wx_app_id"];
    xianLiaoAppid = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"xianliao_app_id"];

    _mapManager = [[BMKMapManager alloc]init];
    NSString *baiduKey = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"baidu_app_key"];
    NSString *urlSchemeName = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"url_schemes_name"];
    BOOL ret = [_mapManager start:baiduKey generalDelegate:self];
    if (!ret) {
        NSLog(@"manager start failed!");
    }
    [SugramApiManager getGameFromSugram:^(NSString *roomToken, NSString *roomId, NSNumber *openId) {
        NSString *gameString = [NSString stringWithFormat:@"%@://?roomToken=%@&roomId=%@&openId=%@",urlSchemeName, roomToken, roomId, openId];
        NSLog(@"%@", gameString);
        UnitySendMessage("NativeReceiver", "OnAutoEnterDesk", gameString.UTF8String);
    }];
    
    [SugramApiManager registerApp:xianLiaoAppid];
    [[MLIAPManager sharedManager] reSendReceip];

    return YES;
}

- (void)applicationWillTerminate:(UIApplication*)application
{
    [super applicationWillTerminate:application];
}

#pragma mark - 第三方应用回调
- (BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url {
    NSLog(@"handleOpenURL url:%@",url);
    if ([SugramApiManager handleOpenURL:url]) {
        return YES;
    }
    return  [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
}
    
- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url  options:(NSDictionary*)options {
    NSLog(@"options openURL url:%@",url);
    if ([SugramApiManager handleOpenURL:url]) {
        return YES;
    }
    NSString *str = [url absoluteString];   //url>string
    NSString *urlSchemeName = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"url_schemes_name"];
    if([str hasPrefix:urlSchemeName])
    {
        UnitySendMessage("NativeReceiver", "OnAutoEnterDesk", str.UTF8String);
        return YES;
    }
    if([str hasPrefix:wxAppId])
    {
        return [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
    }
    
    return [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
}
- (BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation
{
//    NSLog(@"sourceApplication openURL url:%@",url);
//    [super application:application openURL:url sourceApplication:sourceApplication annotation:annotation];
    
    NSLog(@"sourceApplication openURL url:%@",url);
    if ([SugramApiManager handleOpenURL:url]) {
        return YES;
    }
    NSString *str = [url absoluteString];   //url>string
    NSString *urlSchemeName = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"url_schemes_name"];
    if([str hasPrefix:urlSchemeName])
    {
        UnitySendMessage("NativeReceiver", "OnAutoEnterDesk", str.UTF8String);
        return YES;
    }
    
    if([str hasPrefix:wxAppId])
    {
        return [WXApi handleOpenURL:url delegate:[WXApiManager sharedManager]];
    }
    return [super application:application openURL:url sourceApplication:sourceApplication annotation:annotation];
}


- (NSString*)getDeviceVersion
{
    struct utsname systemInfo;
    uname(&systemInfo);
    NSString *deviceVersion = [NSString stringWithCString:systemInfo.machine encoding:NSUTF8StringEncoding];
    return deviceVersion;
}

@end
