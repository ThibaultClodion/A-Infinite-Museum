# A.Infinite Museum

[![Engine](https://img.shields.io/badge/Engine-Unity%206-blue.svg)](https://unity.com/)
[![Language](https://img.shields.io/badge/Language-C%23%20%7C%20Python-green.svg)]()
[![Platform](https://img.shields.io/badge/Platform-OpenXR%20%2F%20VR-orange.svg)]()

An immersive VR research and exploration project designed to investigate human perception of artificial creativity and the attribution of artistic intent. The system procedurally generates infinite, unique museum wings—each curated and hosted by an autonomous, conversational AI persona with a distinct voice, backstory, and aesthetic style.

---

## Key Highlights

* **Procedural Persona & Environment Generation:** Generates comprehensive artist profiles (identity, artistic philosophy, voice parameters, and art-generation prompts) using structured LLM outputs, translating JSON data into modular 3D environments at runtime.
* **Multimodal Conversational Pipeline:** Implements a low-latency voice loop integrating **Wit.ai** (Speech-to-Text), **Gemini API** (dialogue, contextual memory, and persona grounding), and **ElevenLabs** (Text-to-Speech synthesis).
* **Real-Time Spatial & Gaze Awareness:** Leverages raycasting to detect player eye focus/gaze targets in VR, injecting visual context (the specific artwork being viewed) directly into the AI dialogue prompt.
* **Modular Room Loop & Buffer Architecture:** Employs a square-based room transition loop with interstitial survey zones, buffering background generation to eliminate perceptible loading screens.
* **Empirical Data Pipeline:** In-VR interactive survey mechanics coupled with Python-based telemetry scripts for statistical evaluation of user ratings and keyword frequency distributions.

## Getting Started

### Prerequisites

* **Unity:** 2022.3 LTS or Unity 6
* **Hardware:** OpenXR-compatible VR Headset (e.g., Meta Quest 2/3/Pro via Link)
* **API Keys Required:**
  * [Google Gemini API Key](https://aistudio.google.com/)

### Installation & Configuration

1. **Clone the repository:**
   ```bash
   git clone https://github.com/ThibaultClodion/A-Infinite-Museum.git
   ```

2. **Open in Unity:**
   * Open **Unity Hub** and add the cloned folder.
   * Verify that the active build target is set to **Standalone Windows / OpenXR**.

3. **Configure API Credentials:**
   * Update in the Unity editor the file `Assets/Code/Artist/ArtistGeneratorEditor.cs` by entering you Gemini API key into the field box.

4. **Launch the Project:**
   * Open the scene located at `Assets/Scenes/TestScene/TestScene.unity`.
   * Connect your VR headset and press **Play** in the Unity Editor.
---

## Links

* **Portfolio Demo :** [A.Infinite Museum Demo](https://portfolio-thibaultclodion.my.canva.site/portfolio/a-infinite-museum)
