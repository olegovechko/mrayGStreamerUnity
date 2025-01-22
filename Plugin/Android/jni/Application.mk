#APP_ABI := armeabi-v7a
APP_ABI := arm64-v8a

APP_PLATFORM := android-23

APP_STL := c++_shared
APP_CPPFLAGS += -std=c++11
NDK_TOOLCHAIN_VERSION := clang
APP_CPPFLAGS += -frtti
#APP_CPPFLAGS += -fexceptions
