# Unsplash API 配置说明

本应用使用 Unsplash API 来提供动态的城市和天气背景图片。

## 如何配置

### 1. 注册 Unsplash 开发者账号

1. 访问 [Unsplash Developers](https://unsplash.com/developers)
2. 注册或登录你的 Unsplash 账号
3. 创建一个新的应用程序
4. 获取你的 **Access Key**

### 2. 配置 Access Key

在 `WF2/appsettings.json` 文件中，将 `YOUR_UNSPLASH_ACCESS_KEY_HERE` 替换为你的实际 Access Key：

```json
{
  "Unsplash": {
    "AccessKey": "你的_Access_Key_在这里",
    "Comment": "请在 https://unsplash.com/developers 注册并获取你的 Access Key"
  }
}
```

### 3. 重新编译和运行

保存配置文件后，重新编译并运行应用程序。

## 注意事项

- 如果未配置 Unsplash API Key，应用将使用默认的本地背景图片
- Unsplash 免费计划有每小时 50 次请求的限制
- 已获取的图片会被缓存，减少重复请求
- 应用会优雅地降级到本地背景，不会因为 API 失败而崩溃

## 功能说明

配置后，应用将：
- 在首页显示当前城市的风景图片作为背景
- 根据天气条件（晴天、雨天、雪天等）显示相应的天气图片
- 自动缓存已加载的图片，提高性能
