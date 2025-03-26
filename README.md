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
