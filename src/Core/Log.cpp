#include "Core/Log.h"
#include <filesystem>
#include <chrono>
#include <iomanip>
#include <sstream>
#include <windows.h>
#include <Core/PathUtil.h>

namespace EngineCore
{
	LogLevel Log::s_MinLevel = LogLevel::Trace;
	std::ofstream Log::s_LogFile;

	void Log::Init()
	{
		std::string exeDir = GetExecutableDirectory();
		std::string logDir = exeDir + "\\Logs";

		std::filesystem::create_directory(logDir);

		auto now = std::chrono::system_clock::now();
		auto t = std::chrono::system_clock::to_time_t(now);
		
		std::tm tm{};
		localtime_s(&tm, &t);

		std::ostringstream fileName;
		fileName << logDir << "\\engine_"
				 << std::put_time(&tm, "%Y_%m_%d")
				 << ".log";

		s_LogFile.open(fileName.str(), std::ios::app);

		Write(LogLevel::Info, LogCategory::Core, "Engine Started");
	}

	void Log::SetLevel(LogLevel level)
	{
		s_MinLevel = level;
	}

	void Log::Write(LogLevel level, LogCategory category, const std::string& msg)
	{
		if ((int)level < (int)s_MinLevel)
			return;

		auto now = std::chrono::system_clock::now();
		auto t = std::chrono::system_clock::to_time_t(now);
		
		std::tm tm{};
		localtime_s(&tm, &t);

		std::ostringstream line;
		line	<< "[" << std::put_time(&tm, "%H:%M:%S") << "]"
				<< "[" << LevelToString(level) << "]"
				<< "[" << CategoryToString(category) << "] "
				<< msg;

		if (s_LogFile.is_open())
		{
			s_LogFile << line.str() << std::endl;
			s_LogFile.flush();
		}
	}

	//Helper Methods
	const char* Log::LevelToString(LogLevel level)
	{
		switch (level)
		{
		case EngineCore::LogLevel::Trace:
			return "TRACE";
		case EngineCore::LogLevel::Info:
			return "INFO";
		case EngineCore::LogLevel::Warning:
			return "WARNING";
		case EngineCore::LogLevel::Error:
			return "ERROR";
		case EngineCore::LogLevel::Fatal:
			return "FATAL";
		default:
			return "";
		}
	}

	const char* Log::CategoryToString(LogCategory category)
	{
		switch (category)
		{
		case EngineCore::LogCategory::Core:
			return "CORE";
		case EngineCore::LogCategory::Input:
			return "INPUT";
		case EngineCore::LogCategory::Renderer:
			return "RENDERER";
		case EngineCore::LogCategory::Scene:
			return "SCENE";
		case EngineCore::LogCategory::Debug:
			return "DEBUG";
		default:
			return "";
		}
	}	
}