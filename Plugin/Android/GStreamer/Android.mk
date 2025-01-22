LOCAL_PATH := $(call my-dir)
include $(CLEAR_VARS)


#GSTREAMER_ROOT_ANDROID    :=  $(abspath $(LOCAL_PATH)/../armv7)
#SYSROOT  :=  $(abspath $(LOCAL_PATH))


ifndef GSTREAMER_ROOT
ifndef GSTREAMER_ROOT_ANDROID
$(error GSTREAMER_ROOT_ANDROID is not defined!)
endif
ifeq ($(TARGET_ARCH_ABI),arm64-v8a)
    GSTREAMER_ROOT           := $(GSTREAMER_ROOT_ANDROID)/arm64
endif
ifeq ($(TARGET_ARCH_ABI),armeabi-v7a)
    GSTREAMER_ROOT           := $(GSTREAMER_ROOT_ANDROID)/armv7
endif
endif

GSTREAMER_NDK_BUILD_PATH := $(GSTREAMER_ROOT)/share/gst-android/ndk-build

include $(GSTREAMER_NDK_BUILD_PATH)/plugins.mk
GSTREAMER_PLUGINS    := $(GSTREAMER_PLUGINS_CORE) \
                        $(GSTREAMER_PLUGINS_PLAYBACK) \
                        $(GSTREAMER_PLUGINS_CODECS) \
                        $(GSTREAMER_PLUGINS_CODECS_GPL) \
                        $(GSTREAMER_PLUGINS_CODECS_RESTRICTED)\
                        $(GSTREAMER_PLUGINS_NET) \
                        $(GSTREAMER_PLUGINS_NET_RESTRICTED) \
                        $(GSTREAMER_PLUGINS_SYS) \
                        $(GSTREAMER_PLUGINS_EFFECTS) \
                        $(GSTREAMER_PLUGINS_CAPTURE) \
                        $(GSTREAMER_PLUGINS_ENCODING) \
                        $(GSTREAMER_PLUGINS_GES)

#G_IO_MODULES         := gnutls
LOCAL_MODULE         := gstreamer_android
GSTREAMER_EXTRA_DEPS := gstreamer-video-1.0 gstreamer-net-1.0 gstreamer-app-1.0 gstreamer-audio-1.0 gstreamer-base-1.0 gstreamer-tag-1.0 \
						gstreamer-rtsp-1.0 gstreamer-webrtc-1.0 gstreamer-sdp-1.0 libsoup-2.4 glib-2.0
include $(GSTREAMER_NDK_BUILD_PATH)/gstreamer-1.0.mk
