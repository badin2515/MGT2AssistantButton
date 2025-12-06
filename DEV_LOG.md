# MGT2 Assistant Button - Development Log

## Phase 1: Basic Button Foundation (Complete ✅)

### วัตถุประสงค์
สร้างโครงปุ่มพื้นฐานที่สามารถนำไปใช้ในหลายๆ ที่ โดยโคลนมาจากปุ่ม `Button_Desc` ในเมนูสร้างเกม

### ส่วนประกอบที่สร้าง

#### 1. ButtonHelper.cs (`Helpers/ButtonHelper.cs`)
Helper class สำหรับจัดการการสร้างและโคลนปุ่ม ประกอบด้วย:

**Methods:**
- `CloneBasicButton()` - โคลนปุ่มพื้นฐาน (Image + Shadow components only)
- `SetButtonPosition()` - ตั้งค่าตำแหน่งปุ่ม
- `SetButtonSize()` - ตั้งค่าขนาดปุ่ม
- `AddButtonComponent()` - เพิ่ม Button component และกำหนด onClick event
- `AddButtonText()` - เพิ่ม Text child สำหรับข้อความบนปุ่ม

**Features:**
- โคลน RectTransform properties (position, size, anchors, pivot)
- โคลน Image component (sprite, color, material, raycast target)
- โคลน Shadow component (effect color, distance, graphic alpha)
- Logging สำหรับ debugging

#### 2. Patch_Menu_Dev_Game.cs (`Patches/Patch_Menu_Dev_Game.cs`)
Harmony patch สำหรับเมนูสร้างเกม (`Menu_Dev_GameEntwicklungsbericht`)

**Patch Target:**
- `Menu_Dev_GameEntwicklungsbericht.Init()` - Postfix patch

**Functionality:**
- ค้นหาปุ่ม `Button_Desc` จาก menu hierarchy
- โคลนปุ่มพื้นฐานโดยใช้ ButtonHelper
- สร้างปุ่ม "AI Assistant" ใหม่
- ตั้งตำแหน่งด้านล่างของปุ่มต้นฉบับ
- เพิ่ม onClick event handler

**Fallback Logic:**
- ถ้าไม่เจอ `Button_Desc` จะหาปุ่มอื่นที่มี Image + Shadow แทน

### การตั้งค่าโปรเจกต์

#### Dependencies Added:
```xml
<Reference Include="UnityEngine.UI">
  <HintPath>lib\UnityEngine.UI.dll</HintPath>
  <Private>False</Private>
</Reference>
```

#### Build Configuration:
- Target Framework: .NET Framework 4.6
- Auto-copy to BepInEx/plugins after build
- Build output: `bin/Release/net46/MGT2AssistantButton.dll`

### วิธีทดสอบ
1. Build mod: `dotnet build --configuration Release`
2. เริ่มเกม Mad Games Tycoon 2
3. เปิดเมนูสร้างเกม (ห้อง Development)
4. ควรเห็นปุ่ม "AI Assistant" ปรากฏในเมนู
5. ตรวจสอบ log ที่ `BepInEx/LogOutput.log` เพื่อดู debug messages

### Next Steps (TODO)
- [ ] เพิ่ม functionality ให้กับปุ่ม Assistant
- [ ] ออกแบบ UI เมนู Assistant
- [ ] เพิ่มปุ่มในเมนูอื่นๆ (ถ้าต้องการ)
- [ ] ปรับแต่งตำแหน่งและขนาดปุ่มให้เหมาะสม
- [ ] เพิ่ม icon หรือ sprite ให้ปุ่ม

### Files Created:
```
MGT2AssitantButton/
├── Helpers/
│   └── ButtonHelper.cs         # Helper class สำหรับโคลนปุ่ม
├── Patches/
│   └── Patch_Menu_Dev_Game.cs  # Patch เมนูสร้างเกม
└── lib/
    ├── Assembly-CSharp.dll      # Game DLL
    └── UnityEngine.UI.dll       # Unity UI DLL
```

### Build Status
✅ Build successful
✅ Auto-copy to BepInEx/plugins working
✅ No errors or warnings

---

## Phase 1 Fix: Correct Harmony Patch Target (Complete ✅)

### Issue Discovered
Harmony patch ถูกตั้งเป้าไปที่ class ผิด:
- ❌ **Original**: `Menu_Dev_GameEntwicklungsbericht` (เมนูรายงานการพัฒนา)
- ✅ **Fixed**: `Menu_DevGame` (เมนูสร้างเกมหลัก)

### Root Cause
จาก Inspector image path: `CanvasInGameMenu/Menu_Dev_Game/WindowMain/Seite1/`
- ปุ่ม `Button_Desc` อยู่ภายใต้ **`Menu_DevGame`** ไม่ใช่ `Menu_Dev_GameEntwicklungsbericht`
- `Menu_DevGame` = เมนูหลักสำหรับสร้างเกม (3124 บรรทัด)
- `Menu_Dev_GameEntwicklungsbericht` = เมนูรายงานระหว่างพัฒนา (310 บรรทัด)

### Changes Made
**File**: `Patches/Patch_Menu_Dev_Game.cs`
```diff
- [HarmonyPatch(typeof(Menu_Dev_GameEntwicklungsbericht), "Init")]
+ [HarmonyPatch(typeof(Menu_DevGame), "Init")]

- public static void Postfix(Menu_Dev_GameEntwicklungsbericht __instance)
+ public static void Postfix(Menu_DevGame __instance)

- private static GameObject FindButtonDesc(Menu_Dev_GameEntwicklungsbericht menu)
+ private static GameObject FindButtonDesc(Menu_DevGame menu)
```

### Build Result
- ✅ Build successful (0 warnings, 0 errors)
- ✅ Auto-copied to BepInEx/plugins
- ⏳ Pending in-game testing

---

## Technical Notes

### Unity UI Components Used:
- `RectTransform` - ควบคุมตำแหน่งและขนาด
- `CanvasRenderer` - จำเป็นสำหรับ rendering UI
- `Image` - แสดงรูปภาพ/sprite ของปุ่ม
- `Shadow` - เอฟเฟกต์เงาของปุ่ม
- `Button` - ทำให้คลิกได้
- `Text` - ข้อความบนปุ่ม

### Key Coordinates (from Inspector):
- Source Button Position: `(232.6501, 234.61, 0)`
- Assistant Button Position: `(232.65, 184.61, 0)` (50 units below)

### Logging:
ทุกขั้นตอนมี logging เพื่อ debug:
- Button cloning process
- Component copying
- Button creation success/failure
- Click events

---

## Feature Update: Auto-All Button (Complete ✅)

### วัตถุประสงค์
เพิ่มปุ่ม "Auto All" ที่ทุกหน้าของการสร้างเกม (Page 1-5) เพื่อให้ผู้เล่นสามารถกดครั้งเดียวแล้วระบบจะเลือกค่าที่เหมาะสมที่สุดสำหรับทุกหน้า โดยไม่ต้องกลับไปที่หน้าแรก

### Changes Made

#### 1. Core Logic (`Core/AssistantCore.cs`)
- ฟังก์ชัน `ApplyAutoSettings()` (จาก Phase ก่อนหน้า) ทำหน้าที่เลือกค่าที่ดีที่สุดสำหรับทุกหน้า

#### 2. UI Update (`UI/DevGameAssistantUI.cs`)
- ปรับแก้ให้ปุ่ม `Auto All` ถูกสร้างในทุกหน้า (`Seite1` ถึง `Seite5`)
- ตำแหน่ง: `(-80, 193)` บนทุกหน้า
- จัดการเก็บ reference ของปุ่มทั้งหมดใน `List<GameObject> autoAllButtons` เพื่อให้สามารถลบทำความสะอาดได้ถูกต้องเมื่อปิดเมนู

### Verification
- ปุ่มแสดงผลครบทุกหน้า
- การกดปุ่มที่หน้าใดก็ตาม จะส่งผลให้การตั้งค่าของทุกหน้าถูกเลือกอัตโนมัติ
