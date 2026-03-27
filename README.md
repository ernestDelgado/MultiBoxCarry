# MultiBoxCarry (Supermarket Simulator Mod)

A quality-of-life mod that allows players to carry multiple boxes at once, improving late-game workflow without removing the core challenge of inventory management.

---

## 🚀 Features

- Carry up to **6 boxes total** (5 queued + 1 in hand)
- **Queue-based inventory system** for smooth box handling
- **Shift-click support** to grab matching products from racks
- Custom **raycast targeting system** for better interaction control
- Maintains **vanilla behavior** using internal game logic

---

## 🧠 Technical Overview

This project was built using **IL2CPP reverse engineering** and **Harmony patching**.

### Key Implementations

- Hooked into `PlayerInteraction` using Harmony
- Recreated native pickup behavior using a combination of:
  - `PlayerInteraction.Interact()`and PlayerObjectHolder.SetCurrentInteractable()
- Implemented a **custom raycast system** to sees and grabs the multiple boxes.
- Built a **queue-based inventory system** layered on top of the game's existing mechanics
  - simply using the games native system of picking up and dropping a box.
- Fixed vanilla issue where `OnThrow` did not properly clear held objects

---

## 🔍 Reverse Engineering Workflow

To understand and extend the game's systems:

- Used **Il2CppInspectorRedux** to extract:
  - class structures
  - field offsets
  - metadata mappings
- Analyzed native code using **Ghidra**
- Mapped managed structures to native memory layouts
- Traced interaction flow from input → raycast → object handling
- Identified correct internal methods to replicate vanilla behavior safely

---

## ⚙️ Installation

1. Install **BepInEx (IL2CPP version)**
2. Place the compiled DLL into: BepInEx/Plugins

---

## 📌 Notes

This mod focuses on:
- Extending gameplay without breaking immersion
- Preserving original mechanics where possible
- Demonstrating low-level understanding of IL2CPP-based Unity games

---

## 👤 Author

**Ernest Delgado (yaboie88)**  
Brooklyn, NY

Passionate about game systems, modding, and reverse engineering.

---

## ⭐ Support

If you enjoy the mod:
- Leave a **kudos / endorsement**
- It helps support continued development and future opportunities

---

## 🔗 Links

- Nexus Mods: (https://www.nexusmods.com/supermarketsimulator/mods/1495)
- GitHub: https://github.com/ernestDelgado/MultiBoxCarry
