#pragma once
#include "Core/Data/DataLibrary.h"
#include "Core/Data/Animation/AnimationData.h"
#include "Core/Data/Animation/AnimationParser.h"
#include "Core/Data/Interactable/InteractableData.h"
#include "Core/Data/Interactable/InteractableParser.h"
#include "Core/Data/Interactable/TrapData.h"
#include "Core/Data/Interactable/TrapParser.h"
#include "Core/Data/Entity/EntityData.h"
#include "Core/Data/Entity/EntityParser.h"

namespace EnginePlatform
{
	using AnimationLibrary = EngineData::DataLibrary<EngineData::AnimationData, EngineData::AnimationParser>;
	using InteractableLibrary = EngineData::DataLibrary<EngineData::InteractableData, EngineData::InteractableParser>;
	using TrapLibrary = EngineData::DataLibrary<EngineData::TrapData, EngineData::TrapParser>;
	using EntityLibrary = EngineData::DataLibrary<EngineData::EntityData, EngineData::EntityParser>;
}