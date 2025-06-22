# AntiSLCrush

![SCP:SL Plugin](https://img.shields.io/badge/SCP--SL%20Plugin-blue?style=for-the-badge)
![Language](https://img.shields.io/badge/Language-C%23-blueviolet?style=for-the-badge)
![Downloads](https://img.shields.io/github/downloads/angelseraphim/AntiSLCrush/total?label=Downloads&color=333333&style=for-the-badge)

---
AntiSLCrush is a plugin for SCP: Secret Laboratory using LabApi, designed to prevent server crashes caused by game bugs, developer errors, or DDoS attacks.
It adds additional protection and stability to your server.
---

## 📦 Installation

1. Download the latest release from the [Releases](../../releases) page.
2. Place the `.dll` file into your server's **LabApi** plugin directory.
3. Configure `config.yml` as needed.
4. Install `iptables` (required for traffic filtering):

```bash
sudo apt install iptables
```

---

## ⚠️ Important Notes ⚠️

* This plugin **requires a Linux-based operating system** (e.g., Ubuntu, Debian).
* The plugin **requires sudo privileges** to function correctly. If your server is running without elevated permissions, **the plugin will not work correctly**.
**If you are unable to follow these steps, disable ``ban_hex`` and ``ban_ip`` in the config**

* The code is open and license-free — feel free to reuse or modify parts of it as needed.
