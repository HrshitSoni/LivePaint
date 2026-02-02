# LivePaint

LivePaint is a simple real-time collaborative drawing app built with **.NET**.  
It has a client application for drawing and a server that keeps all connected users in sync.

This project was mainly built to explore real-time communication and shared drawing state.

---

## Project Structure

- **LivePaint** – client-side app (UI + drawing logic)
- **LivePaint.Server** – server that handles connections and broadcasts updates
- **DrawModel.cs** – shared model used to represent drawing data

---

## Features

- Real-time drawing with multiple users
- Client–server architecture
- Shared drawing model
- Built using C# and .NET
