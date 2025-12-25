#pragma once
#include <string>
#include <fstream>

namespace EngineCore 
{
	enum class LogLevel
	{
		Trace,
		Info,
		Warning,
		Error,
		Fatal
	};

	enum class LogCategory
	{
		Core,
		Input,
		Renderer,
		Scene,
		Debug
	};

	class Log
	{
	public :
		static void Init();
		static void SetLevel(LogLevel level);
		static void Write(LogLevel level, LogCategory category, const std::string& msg);
	private:
		static const char* LevelToString(LogLevel level);
		static const char* CategoryToString(LogCategory category);
		static LogLevel s_MinLevel;
		static std::ofstream s_LogFile;
	};
}