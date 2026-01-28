#include <iostream>
#include <string>
#include "kernel_utils.h"
#include "pattern.h"

int main()
{
	auto base_addr = KernelUtils::ntoskrnl_base();

	//auto SeValidateImageHeader_offset = KernelUtils::get_sevalidateimageheader_offset();
	//auto SeValidateImageData_offset = KernelUtils::get_sevalidateimagedata_offset();
	//auto offset_ret = KernelUtils::get_ret_offset();
	auto patchguard_value_offset = KernelUtils::get_patchguardvalue_offset();
	auto PatchGuard_offset = KernelUtils::get_patchguard_offset();
}