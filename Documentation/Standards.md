# UFeel – Coding Standards

UFeel follows a hybrid set of standards inspired by:

- **UFeel internal guidelines**
- **Unity best practices**

---

# 1. Code Standards

### C#
- **PascalCase** for classes/methods, **camelCase** for variables, `_camelCase` for private serialized fields.
- Group code with `#region` blocks (Unity Methods, Public API, Private Methods).
- One responsibility per script.
- Avoid allocations inside `Update()` and use `TryGetComponent` when possible.
- Constants: `UPPER_CASE`
- Events: `OnSomethingHappened`

### Python
- Follow **PEP 8**: `snake_case` for functions/variables, `PascalCase` for classes.
- Use **type hints** and meaningful docstrings.
- Keep modules focused (server, models, utils).
- Avoid long functions (>80 lines) and unused imports.
- Use `logging` instead of `print()` for production.

### C++ (to do)

---

# 2. File Organization

- Each class in its own file
- Demo code separated from plugin code
- API code kept lightweight and documented

---

# 3. Commenting / Documentation (to do)

- Documentation for each scene, each model (emotion, eye tracking, speech to text, biometric sensor)
- Documentation of Testing room

---

# 4. Unity Best Practices

- Clear folder separation: `Scripts/`, `Scenes/`, `Prefabs/`, `Plugins/`, `Resources/`.
- One scene = one purpose.
- No logic in constructors — use `Awake()` and `Start()`.
- Use **ScriptableObjects** for shared data when appropriate.
- Keep assets organized, no unused files, and create reusable objects as **Prefabs**.
- Avoid `FindObjectOfType`
- Minimize update loops
- Use event-driven patterns when possible
- Keep scenes modular

---

# 5. Internal Rules (to see later)

- No hard-coded values


---

# 6. Code Review Standard

Each Pull Request must:

- Respect naming conventions
- Avoid performance issues
- Be tested

