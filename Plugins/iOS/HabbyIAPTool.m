
#import <Foundation/Foundation.h>
#import "HabbyIAPTool.h"

#if !MAC_APPSTORE
#import "UnityEarlyTransactionObserver.h"
#endif
char* _GetCountryCode()
{
    const char* tstr = [HabbyIAPTool GetCountryCode];
    if(tstr == NULL) return NULL;
    char* res = (char*)malloc(strlen(tstr)+1);
    strcpy(res, tstr);
    return res;
}

@implementation HabbyIAPTool
+(const char*)GetCountryCode
{
    if (@available(iOS 13.0, *)) {
        SKStorefront * tfront = [[SKPaymentQueue defaultQueue] storefront];
        NSString * tcode = tfront.countryCode;
        return [tcode UTF8String];
    } else {
        return NULL;
    }
}
@end
