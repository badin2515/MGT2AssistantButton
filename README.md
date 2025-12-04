# MGT2 Assistant Button Mod

BepInEx mod สำหรับ Mad Games Tycoon 2 เพื่อเพิ่มฟีเจอร์ปุ่มช่วยเหลือในเกม

## ความต้องการ

- BepInEx 5.x
- Mad Games Tycoon 2
- .NET Framework 4.6

## การติดตั้ง

1. ติดตั้ง BepInEx ในโฟลเดอร์เกม Mad Games Tycoon 2
2. คัดลอกไฟล์ `MGT2AssistantButton.dll` ไปที่ `BepInEx/plugins/`
3. เริ่มเกม

## การพัฒนา

### เตรียมโปรเจกต์

1. สร้างโฟลเดอร์ `lib` ในโปรเจกต์
2. คัดลอกไฟล์ DLL จากเกม:
   - `Assembly-CSharp.dll` จาก `Mad Games Tycoon 2_Data/Managed/`

### Build

```bash
dotnet build --configuration Release
```

### ติดตั้งสำหรับทดสอบ

หลัง build เสร็จ ไฟล์ DLL จะอยู่ที่ `bin/Release/net46/MGT2AssistantButton.dll`
คัดลอกไปที่ `BepInEx/plugins/` ในโฟลเดอร์เกม

## โครงสร้างโปรเจกต์

```
MGT2AssitantButton/
├── Plugin.cs              # Main plugin entry point
├── PluginInfo.cs          # Plugin metadata
├── Patches/               # Harmony patches
│   └── ExamplePatch.cs   # ตัวอย่าง patch
├── lib/                   # Game DLLs (ไม่ commit ใน git)
│   └── Assembly-CSharp.dll
└── MGT2AssistantButton.csproj
```

## License

MIT
