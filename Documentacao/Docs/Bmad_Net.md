# Bmad - Full Stack Agent Methodology

## 1. Principles

* **English ONLY** for code (variables, functions, classes, comments).
* **Portuguese ONLY** for business domain terms in documentation/strings (e.g., `Cliente`, `Pedido`).
* **Clean Architecture**: Separation of concerns.
* **Micro-Frontends**: Preference for Module Federation when multiple teams/modules interact.

## 2. Frontend Architecture

* **Module Federation**: Used for splitting large apps.
  * **Shell**: Main application container.
  * **Remote**: Feature modules exposed via Webpack.
* **Stand-alone Components**: Preferred for Angular > 15.
* **Folder Structure**:
  * `features/`: Domain specific modules.
  * `shared/` or `library/`: Reusable logs, auth, UI types.

## 3. Directory Structure

* `Frontend/Mobile/Angular`: Main Shell / Monoliths.
* `Frontend/Modules`: Micro-frontends (Remotes).
* `Frontend/Library`: npm packages.
* `Frontend/Web/React`: (Example/Expansion).
* `Frontend/Mobile/Flutter`: (Example/Expansion).

## 4. Updates

* **2026-02-10**: Added support for Module Federation workflows.
