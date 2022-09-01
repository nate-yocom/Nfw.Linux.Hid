#!/bin/bash

# Originally from: https://github.com/tiny-pilot/ansible-role-tinypilot/blob/master/files/lib/usb-gadget.sh
# Minor modifications to customize for our usage

export readonly USB_DEVICE_DIR="g1"
export readonly USB_GADGET_PATH="/sys/kernel/config/usb_gadget"
export readonly USB_DEVICE_PATH="${USB_GADGET_PATH}/${USB_DEVICE_DIR}"

export readonly USB_STRINGS_DIR="strings/0x409"
export readonly USB_KEYBOARD_FUNCTIONS_DIR="functions/hid.keyboard"
export readonly USB_MOUSE_FUNCTIONS_DIR="functions/hid.mouse"
export readonly USB_JOYSTICK_FUNCTIONS_DIR="functions/hid.joystick"

export readonly USB_CONFIG_INDEX=1
export readonly USB_CONFIG_DIR="configs/c.${USB_CONFIG_INDEX}"
export readonly USB_ALL_CONFIGS_DIR="configs/*"
export readonly USB_ALL_FUNCTIONS_DIR="functions/*"

function usb_gadget_activate {
  ls /sys/class/udc > "${USB_DEVICE_PATH}/UDC"
  chmod 777 /dev/hidg0
  chmod 777 /dev/hidg1
  chmod 777 /dev/hidg2
}
export -f usb_gadget_activate

function usb_gadget_deactivate {
  echo '' > "${USB_DEVICE_PATH}/UDC"
}
export -f usb_gadget_deactivate