# doky-RPG-FPS

Unity 第一人称 RPG 射击可运行原型（无需额外美术资源）。

## 当前状态
- 使用最简单几何体（Capsule/Sphere/Plane）自动生成玩家、敌人、地图和强化点。
- 已实现：
  - RPG 射击循环
  - 装备系统（白/蓝/紫/金）
  - 击杀得分与关卡/无尽强化
  - 无尽模式每 1 分钟敌人强化
  - 每 20 秒刷新强化点（加血量上限/护甲/攻击/射速）
  - 合作 PvE 面板：每人造成伤害、承伤、击杀

## 运行
1. 用 Unity 2022 LTS+ 打开 `UnityProject`
2. 打开任意场景点击 Play（脚本会自动创建运行场景）

详细文档：`Docs/UnityImplementationGuide.zh-CN.md`
