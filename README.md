# one-driver-power-supply-basic

**OneDriver.PowerSupply.Basic** is a concrete implementation of the abstract `CommonDevice` defined in the `OneDriver.PowerSupply.Abstract` package. This power supply driver provides basic functionality to control either voltage or current depending on the selected control mode.

## 🔧 Features

- Supports both **voltage control** and **current control** modes
- Allows setting:
  - Output voltage (in current Control Mode)
  - Output current (in voltage Control Mode)
- Channel-based control: each output channel can be configured independently
- Built-in validation and connection management
- Easy integration with other OneDriver Framework components

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Reference to `OneDriver.PowerSupply.Abstract` package
- Optional: Serilog for logging (already used in base classes)

### Installation

Install via NuGet:

```bash
dotnet add package OneDriver.PowerSupply.Basic

Sure! Here's your gRPC command usage section written in clean, Markdown-style markup that you can drop directly into a `README.md`:

---

## 🔌 Power Supply Control via `grpcurl`

Use `grpcurl` to control and monitor your power supply via the gRPC server running at `localhost:5248`.

### ✅ Prerequisites
- [grpcurl](https://github.com/fullstorydev/grpcurl/releases) installed
- Server running at `http://localhost:5248`
- Power supply connected to the correct COM port (e.g., `COM5`)

---

### 🔗 Open Connection

```bash
grpcurl -plaintext -d "{\"port\": \"COM5\"}" localhost:5248 power.PowerSupply/OpenConnection
```

> Opens a serial connection to the power supply on COM5.

---

### ⚡ Set Voltage

```bash
grpcurl -plaintext -d "{\"channel\": 0, \"value\": 7.92}" localhost:5248 power.PowerSupply/SetVolts
```

> Sets the voltage on channel 0 to **7.92V**.

---

### 📡 Stream Live Process Data

```bash
grpcurl -plaintext -d "{\"channelNumber\": 0}" localhost:5248 power.PowerSupply/StreamProcessData
```

> Streams real-time voltage and current data from **channel 0**.

---

