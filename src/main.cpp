#include "Core/Application.h"
#include "Core/Log.h"

int main(int argc, char** argv)
{
    EngineCore::Application app;

    /*
    EngineCore::Log::Write(
        EngineCore::LogLevel::Info,
        EngineCore::LogCategory::Debug,
        "argc = " + std::to_string(argc)
    );

    for (int i = 0; i < argc; i++)
    {
        EngineCore::Log::Write(
            EngineCore::LogLevel::Info,
            EngineCore::LogCategory::Debug,
            std::string("argv[") + std::to_string(i) + "] = " + argv[i]
        );
    }*/
   
	app.Run();
    return 0;
}