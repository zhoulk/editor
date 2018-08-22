#import "UnityAppController.h"
#import <BaiduMapAPI_Base/BMKBaseComponent.h>

@interface MyUnityAppController : UnityAppController<BMKGeneralDelegate>
{
   BMKMapManager *_mapManager;
    
    
}

@end
