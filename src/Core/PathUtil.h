#pragma once
#include <string>

namespace EngineCore
{
	std::string GetExecutableDirectory();
	std::string GetRootDirectory();
	std::string GetFile(std::string folderName, std::string fileName);
}