# WF2 测试项目说明

## 测试结构

本测试项目使用 NUnit 测试框架和 Moq mocking 库，实现了对核心服务和 ViewModel 的全面测试覆盖。

### 项目架构符合要求

1. **View 和 ViewModel 层**
   - `MainView` + `MainViewModel`
   - `WeatherDetailView` + `WeatherDetailViewModel`
   - `CitiesView` + `CitiesViewModel`

2. **Service1 层（可测试的服务）**
   - `ISettingsService` - 设置服务
     - 功能1: 保存/获取语言设置
     - 功能2: 保存/获取主题设置
     - 功能3: 保存/获取上次选择的城市
     - 功能4: 保存/获取背景图片路径

   - `IWeatherCacheService` - 天气缓存服务
     - 功能1: 保存和获取天气数据
     - 功能2: 管理收藏城市列表
     - 功能3: 更新收藏状态
     - 功能4: 删除和清空缓存

3. **Service2 层（不可测试的服务）**
   - LiteDB 数据库 - 作为持久化存储，直接依赖文件系统和第三方库

4. **依赖注入**
   - 所有服务通过构造函数注入
   - 使用接口定义服务契约
   - 测试中使用 Moq 创建 mock 对象

## 测试文件

### Services 测试
- `SettingsServiceTests.cs` - 测试设置服务的所有功能
  - 语言设置的保存和获取
  - 主题设置的保存和获取
  - 城市选择的保存和获取
  - 背景图片路径的保存和获取
  - 设置覆盖功能
  - 多个独立设置的维护

- `WeatherCacheServiceTests.cs` - 测试天气缓存服务
  - 天气数据的保存和获取
  - 数据覆盖更新
  - 获取所有城市列表
  - 获取收藏城市列表
  - 更新收藏状态
  - 删除天气数据
  - 清空所有缓存
  - WeatherCache.FromApiResponse 转换

### ViewModels 测试
- `MainViewModelTests.cs` - 测试主视图模型
  - 初始化和设置加载
  - 缓存数据显示
  - 侧边栏切换
  - 语言和主题更新
  - 天气图标映射
  - 收藏状态保持

- `WeatherDetailViewModelTests.cs` - 测试天气详情视图模型
  - 城市名称设置
  - 收藏功能（添加/警告）
  - 城市搜索
  - 缓存加载
  - Toast 通知显示
  - 收藏状态检查
  - 语言更新

- `CitiesViewModelTests.cs` - 测试城市列表视图模型
  - 收藏城市列表加载
  - 空列表处理
  - 删除城市
  - 选择城市并保存
  - 刷新城市列表
  - 错误处理
  - 语言更新

## 运行测试

### 使用 .NET CLI
```bash
cd WF2UTest
dotnet test
```

### 使用 Visual Studio / Rider
1. 打开测试资源管理器
2. 点击"运行所有测试"

### 生成覆盖率报告
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 测试覆盖率

- **SettingsService**: ~100% 覆盖所有可测试方法
- **WeatherCacheService**: ~100% 覆盖所有可测试方法
- **ViewModels**: 覆盖主要业务逻辑和命令

## 注意事项

1. 所有测试都是隔离的，使用临时数据库文件
2. 每个测试后都会清理测试数据
3. 使用 Moq 框架模拟依赖服务
4. 测试覆盖了正常流程和异常情况
5. LiteDB 作为 Service2 层不直接测试，通过 Service1 的集成测试间接验证

## Mock 对象使用

测试中使用 Moq 创建以下 mock 对象：
- `Mock<ISettingsService>` - 模拟设置服务
- `Mock<IWeatherCacheService>` - 模拟缓存服务
- `Mock<ILocalizationService>` - 模拟本地化服务
- `Mock<IBackgroundImageService>` - 模拟背景图片服务

这些 mock 对象允许我们：
- 隔离被测试组件
- 控制依赖服务的行为
- 验证方法调用
- 模拟异常情况
