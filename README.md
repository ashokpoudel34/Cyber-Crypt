# Cyber Crypt

Cyber Crypt is a GUI-based encryption and injector tool designed for offensive security research. It allows you to:
- Encrypt a `.exe` file into a `.bin` format
- Generate a final `.exe` that decrypts and injects the payload into a target process

> ⚠️ For educational and authorized security research purposes only.

---

🚀 Features

- ✅ Payload Encryption: Convert `.exe` payloads into encrypted `.bin` format using custom AES logic.
- 🏗 Dynamic Loader Generation: Build a final injector `.exe` using an obfuscated and optimized template with target process injection logic.
- 🔒 Direct Syscalls & Anti-Analysis: Template code leverages direct syscalls and avoids typical API call patterns.
- 📦 Single Executable Output: Generate compact, standalone loaders using Native AOT (no .NET runtime dependencies).
- 🧬 Behavior Randomization: Loader logic supports injection randomness and delay options to avoid heuristic detection.


---

## 🛠️ How It Works

1. **Encrypt Payload:**
   - Load a `.exe` file
   - Output is an encrypted `.bin` file (AES ECB with no padding)

2. **Build Injector:**
   - Load encrypted `.bin` and provide a target process name
   - Output is a new `.exe` injector compiled from a stealthy template

3. **Injection:**
   - The generated injector starts the target process (e.g., `notepad.exe`)
   - Adds it to Defender exclusion list
   - Decrypts and injects shellcode via direct syscalls (NtOpenProcess, NtAllocateVirtualMemory, etc.)

---

## ⚠️ Disclaimer

This tool is intended solely for **educational** and **ethical penetration testing** in **authorized environments**. Misuse can lead to legal consequences. The author is not responsible for any damage caused by unauthorized use.

---

💬 Credits

- Coded with ❤️ using .NET
- Inspired by real-world evasion techniques and stealthy injection frameworks
