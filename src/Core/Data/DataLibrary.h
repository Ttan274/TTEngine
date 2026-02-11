#pragma once
#include <Core/JsonLoader.h>
#include <unordered_map>
#include <filesystem>

namespace EngineData
{
	template<typename T, typename Parser>
	class DataLibrary
	{
	public:
		//Load all json files in a folder
		static bool LoadFromFolder(const std::string& path)
		{
			for (auto& file : std::filesystem::directory_iterator(path))
			{
				if (file.path().extension() != ".json")
					continue;

				nlohmann::json j;
				if (!EngineCore::JsonLoader::LoadFromFile(file.path().string(), j))
					continue;

				T data = Parser::Parse(j);

				if (data.id.empty())
					continue;

				if (s_Data.contains(data.id))
				{
					//duplicate warning
				}

				s_Data[data.id] = data;
			}

			return true;
		}

		//Load all datas in a json file
		static bool LoadFromFile(const std::string& path)
		{
			nlohmann::json j;
			if (!EngineCore::JsonLoader::LoadFromFile(path, j))
				return false;

			if (!j.is_array())
				return false;

			for (auto& item : j)
			{
				T data = Parser::Parse(item);

				if (data.id.empty())
					continue;

				s_Data[data.id] = data;
			}

			return true;
		}

		static const T* Get(const std::string& id)
		{
			auto it = s_Data.find(id);
			if (it != s_Data.end())
				return &it->second;
			return nullptr;
		}

		static void Clear() { s_Data.clear(); }

	private:
		static inline std::unordered_map<std::string, T> s_Data;
	};
}