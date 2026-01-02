#pragma once
#include <string>
#include <windows.h>
#include <filesystem>

namespace EngineCore
{
	static std::string GetExecutableDirectory()
	{
		char buffer[MAX_PATH];
		GetModuleFileNameA(nullptr, buffer, MAX_PATH);

		std::string fullPath(buffer);
		size_t lastSlash = fullPath.find_last_of("\\/");
		return fullPath.substr(0, lastSlash);
	}

	static std::string GetRootDirectory()
	{
		std::string exeDir = GetExecutableDirectory();
		std::filesystem::path root = std::filesystem::path(exeDir).parent_path().parent_path();

		std::string mapPath = root.string();

		return mapPath;
	}


	static std::string GetFile(std::string folderName, std::string fileName)
	{
		std::string rootDir = GetRootDirectory() + "\\Assets";
		std::filesystem::path asset = std::filesystem::path(rootDir);

		std::string targetPath = (asset / folderName / fileName).string();

		return targetPath;
	}
}