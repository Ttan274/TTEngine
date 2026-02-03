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

	std::string GetRootDirectory()
	{
		std::string exeDir = GetExecutableDirectory();
		std::filesystem::path root = std::filesystem::path(exeDir).parent_path().parent_path();

		std::string mapPath = root.string();

		return mapPath;
	}


	std::string GetFile(std::string folderName, std::string fileName)
	{
		std::string rootDir = GetRootDirectory() + "\\Assets";
		std::filesystem::path asset = std::filesystem::path(rootDir);

		std::string targetPath = (asset / folderName / fileName).string();

		return targetPath;
	}
}