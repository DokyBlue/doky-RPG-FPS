# 第一人称 RPG 射击（Unity，可直接运行）

你现在可以在**没有任何美术资源**的情况下直接运行原型：
- 地图、玩家、敌人、强化点全部由 **Unity Primitive**（Plane/Capsule/Sphere）动态生成。
- 支持 RPG 射击、装备品质、敌人强化、无尽模式、合作 PvE 伤害/承伤统计。

## 快速运行
1. 用 Unity 2022 LTS+ 打开 `UnityProject`。
2. 打开任意场景（甚至空场景）点击 Play。
3. `PrimitiveGameBootstrap` 会在运行前自动创建所有系统和几何体场景。

## 操作
- `WASD`：移动
- 鼠标：视角
- 鼠标左键：射击
- `Esc`：释放鼠标

## 已覆盖需求

### 1) RPG 射击
- `FpsPlayerController` + `ProjectileWeapon` + `HealthComponent` 组成第一人称射击闭环。

### 2) 装备系统（防御、血量上限、攻速）+ 品质
- `EquipmentDefinition`：白/蓝/紫/金品质倍率。
- `EquipmentLoadout`：重算并应用玩家属性。

### 3) 击杀得分 + 通关/结算
- 敌人死亡加分（`EnemyRuntime` -> `ScoreManager`）。
- `GameModeController` 负责模式计时与胜利条件。

### 4) 过关后敌人强化（血量/护甲/攻击）
- `EnemyLevelProfile` 定义 level0 与每级成长。
- `EnemyDirector` 根据当前 level 生成强化敌人。
- Level0 默认 1 分，等级越高分数越高。

### 5) 无尽模式
- 每 1 分钟敌人自动强化（`GameModeController`）。
- 每 20 秒随机刷新强化点（`BuffPointSpawner`）。
- 强化点提供：血量上限/护甲/攻击/射速。
- `EndlessResultRecorder` 可输出坚持时间和分数。

### 6) 联机模式（合作 PvE 统计面板）
- `CoopDamageTracker` 统计每位玩家：造成伤害 / 承受伤害 / 击杀。
- HUD (`SimpleHudPresenter`) 实时展示每人数据。
- 当前原型默认创建 P1（本地玩家）和 P2（队友占位），便于验证统计流程；后续可直接接入 Netcode/Mirror 用真实网络玩家替换。

## 关键脚本
- 自动开局搭建：`Assets/Scripts/Gameplay/PrimitiveGameBootstrap.cs`
- 玩家与射击：`Assets/Scripts/Player/FpsPlayerController.cs`、`Assets/Scripts/Combat/ProjectileWeapon.cs`
- 敌人与生成：`Assets/Scripts/Spawning/EnemySpawner.cs`、`Assets/Scripts/AI/SimpleEnemyAI.cs`
- 模式与强化：`Assets/Scripts/Core/GameModeController.cs`、`Assets/Scripts/Progression/EnemyLevelProfile.cs`
- 强化点：`Assets/Scripts/Spawning/BuffPointSpawner.cs`
- 合作统计与 UI：`Assets/Scripts/Network/CoopDamageTracker.cs`、`Assets/Scripts/UI/SimpleHudPresenter.cs`
