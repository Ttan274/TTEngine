#pragma once
#include "Core/Data/DataLibrary.h"
#include "Core/Data/Animation/AnimationData.h"
#include "Core/Data/Animation/AnimationParser.h"

namespace EnginePlatform
{
	using AnimationLibrary = EngineData::DataLibrary<EngineData::AnimationData, EngineData::AnimationParser>;
}