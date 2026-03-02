# 第一人称 RPG 射击（Unity）实现说明

本仓库提供一套可直接落地到 Unity（推荐 2022 LTS+）的脚本骨架，覆盖你提出的 6 条需求。

## 1) RPG 射击基础
- 核心血量/护甲/攻击/射速字段使用 `StatBlock`。
- `HealthComponent` 提供伤害结算（护甲减伤）与死亡事件。
- `ScoreManager` 负责统一记分。

## 2) 装备系统（防御、血量上限、攻速）
- `EquipmentDefinition` 定义装备与品质（白/蓝/紫/金）。
- `EquipmentLoadout` 负责装备穿戴后重算角色属性。
- 品质倍率：白 1.0、蓝 1.2、紫 1.45、金 1.75，可在脚本中继续平衡。

## 3) 击杀得分 + 通关条件
- `EnemyRuntime` 在敌人死亡时调用 `ScoreManager.AddScore`。
- `GameModeController` 支持两类通关：
  - 坚持时间达到阈值
  - 分数达到阈值

## 4) 关卡推进后敌人强化
- `EnemyLevelProfile` 定义 Level0 基准与每级成长倍率（血量、护甲、攻击、得分）。
- `EnemyDirector.NextStage()` 用于每过一关提升敌人等级。
- Level0 敌人默认 1 分，后续等级按倍率提高。

## 5) 无尽模式
- `GameModeController` 在无尽模式每 60 秒调用强化逻辑。
- `BuffPointSpawner` 每 20 秒刷新强化点（拾取后增加：血量上限、护甲、攻击、射速）。
- `EndlessResultRecorder` 记录并返回无尽模式存活时间和得分。

## 6) 联机合作 PvE（每人伤害/承伤）
- `CoopDamageTracker` 记录玩家造成伤害与承受伤害。
- 可接入 Netcode for GameObjects / Mirror：
  1. 服务器权威下在伤害判定点调用 `RecordDamageDealt`。
  2. 玩家受击时调用 `RecordDamageTaken`。
  3. 在 UI 排行板中读取 `DealtByPlayer`、`TakenByPlayer` 展示。

---

## 场景接线建议
1. 新建 `GameSystems` 空物体，挂载：
   - `ScoreManager`
   - `GameModeController`
   - `EnemyDirector`
   - `BuffPointSpawner`（无尽模式启用）
   - `CoopDamageTracker`（联机模式启用）
2. 玩家预制体挂载：
   - `HealthComponent`
   - `EquipmentLoadout`
3. 敌人预制体挂载：
   - `HealthComponent`
   - `EnemyRuntime`
4. 用 ScriptableObject 创建：
   - `EnemyLevelProfile`
   - 多个 `EquipmentDefinition`

## 下一步建议
- 增加武器系统（命中扫描/弹道）并统一走 `DamageInfo`。
- 增加掉落表：按品质概率掉落装备。
- 增加局外养成（解锁天赋或永久加成）。
