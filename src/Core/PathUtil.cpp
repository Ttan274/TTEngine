#include "Core/PathUtil.h"

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <windows.h>

#include <filesystem>

namespace EngineCore
{
	std::string GetExecutableDirectory()
	{
		char buffer[MAX_PATH];
		GetModuleFileNameA(nullptr, buffer, MAX_PATH);

		std::string fullPath(buffer);
		size_t lastSlash = fullPath.find_last_of("\\/");
		return fullPath.substr(0, lastSlash);
	}

	std::string GetFile(std::string folderName, std::string fileName)
	{
		std::string rootDir = GetExecutableDirectory() + "\\Assets";
		std::filesystem::path asset = std::filesystem::path(rootDir);

		std::string targetPath = (asset / folderName / fileName).string();

		return targetPath;
	}
}