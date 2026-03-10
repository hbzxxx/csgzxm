using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class VersionEditor : MonoBehaviour
{
    [MenuItem("版本控制/打开本地目录")]
    private static void OpenPersistentDataPath()
    {
        string path = Application.persistentDataPath.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", path);
    }

    [MenuItem("版本控制/导表 (Luban)")]
    private static void ExportLubanTables()
    {
        if (!RunLuban())
        {
            return;
        }

        AssetDatabase.Refresh();

        // 导表成功后，自动执行复制和生成 version 文件
        try
        {
            CopyTableAndGenerateVersionInternal();
            EditorUtility.DisplayDialog("导表完成", "Luban 导表并生成 version 文件成功！", "确定");
        }
        catch (Exception e)
        {
            Debug.LogError($"[VersionEditor] 生成 version 文件失败: {e.Message}");
            EditorUtility.DisplayDialog("导表完成但 version 生成失败", $"Luban 导表成功，但生成 version 文件失败：\n{e.Message}", "确定");
        }
    }

    /// <summary>
    /// 通过 cmd 调用 Luban bat 文件进行导表
    /// </summary>
    private static bool RunLuban()
    {
        var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        var batPath = Path.Combine(projectRoot, "Luban/gen_code_json.bat");

        if (!File.Exists(batPath))
        {
            var errorMsg = $"未找到 Luban 批处理文件：\n{batPath}";
            Debug.LogError($"[VersionEditor] {errorMsg}");
            EditorUtility.DisplayDialog("Luban 导表失败", errorMsg, "确定");
            return false;
        }

        try
        {
            // 通过 cmd /c 调用 bat，chcp 65001 确保 UTF-8 输出
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c chcp 65001 >nul && \"{batPath}\"",
                WorkingDirectory = Path.GetDirectoryName(batPath),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using (var process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    var errorMsg = "无法启动 Luban 进程";
                    Debug.LogError($"[VersionEditor] {errorMsg}");
                    EditorUtility.DisplayDialog("Luban 导表失败", errorMsg, "确定");
                    return false;
                }

                var stdout = process.StandardOutput.ReadToEnd();
                var stderr = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(stdout))
                {
                    Debug.Log($"[VersionEditor] Luban 输出:\n{stdout}");
                }

                if (!string.IsNullOrEmpty(stderr))
                {
                    Debug.LogWarning($"[VersionEditor] Luban 警告:\n{stderr}");
                }

                // 检测失败
                var isFailByExitCode = process.ExitCode != 0;
                var isFailByOutput = !string.IsNullOrEmpty(stdout) && stdout.Contains("== fail ==");
                var isFailByStderr = !string.IsNullOrEmpty(stderr) &&
                                     (stderr.Contains("You must install or update .NET") ||
                                      stderr.Contains("install missing framework"));

                if (isFailByExitCode || isFailByOutput || isFailByStderr)
                {
                    var parsedError = ExtractLubanError(stdout);
                    var errorBuilder = new StringBuilder();

                    if (isFailByStderr)
                    {
                        errorBuilder.AppendLine("Luban 执行环境错误");
                    }
                    else if (isFailByExitCode)
                    {
                        errorBuilder.AppendLine($"Luban 执行失败，退出码：{process.ExitCode}");
                    }
                    else
                    {
                        errorBuilder.AppendLine("Luban 导表失败");
                    }

                    if (isFailByStderr && !string.IsNullOrEmpty(stderr))
                    {
                        errorBuilder.AppendLine();
                        errorBuilder.AppendLine(TruncateForDialog(stderr, 1500));
                    }
                    else if (!string.IsNullOrEmpty(parsedError))
                    {
                        errorBuilder.AppendLine();
                        errorBuilder.AppendLine(parsedError);
                    }
                    else if (!string.IsNullOrEmpty(stderr))
                    {
                        errorBuilder.AppendLine();
                        errorBuilder.AppendLine("错误信息：");
                        errorBuilder.AppendLine(TruncateForDialog(stderr, 1500));
                    }
                    else if (!string.IsNullOrEmpty(stdout))
                    {
                        errorBuilder.AppendLine();
                        errorBuilder.AppendLine("输出信息：");
                        errorBuilder.AppendLine(TruncateForDialog(stdout, 1500));
                    }

                    var errorMsg = errorBuilder.ToString();
                    Debug.LogError($"[VersionEditor] {errorMsg}");
                    EditorUtility.DisplayDialog("Luban 导表失败", errorMsg, "确定");
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            var errorMsg = $"Luban 执行异常：{e.Message}";
            Debug.LogError($"[VersionEditor] {errorMsg}");
            EditorUtility.DisplayDialog("Luban 导表失败", errorMsg, "确定");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 从 Luban 输出中提取详细错误信息
    /// </summary>
    private static string ExtractLubanError(string stdout)
    {
        if (string.IsNullOrEmpty(stdout))
        {
            return null;
        }

        var failIndex = stdout.IndexOf("GenJob fail", StringComparison.Ordinal);
        if (failIndex < 0)
        {
            return null;
        }

        const string separator = "=======================================================================";
        var firstSepIndex = stdout.IndexOf(separator, failIndex, StringComparison.Ordinal);
        if (firstSepIndex < 0)
        {
            return TruncateForDialog(stdout.Substring(failIndex), 1000);
        }

        var secondSepIndex = stdout.IndexOf(separator, firstSepIndex + separator.Length, StringComparison.Ordinal);
        if (secondSepIndex < 0)
        {
            return TruncateForDialog(stdout.Substring(firstSepIndex), 1000);
        }

        var errorContent = stdout.Substring(firstSepIndex, secondSepIndex + separator.Length - firstSepIndex);
        return TruncateForDialog(errorContent.Trim(), 1000);
    }

    /// <summary>
    /// 截断过长文本以适应弹窗显示
    /// </summary>
    private static string TruncateForDialog(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
        {
            return text;
        }

        return text.Substring(0, maxLength) + "\n\n... (内容过长已截断，请查看 Console 日志获取完整信息)";
    }
 

    /// <summary>
    /// 复制表格到持久化目录并生成 version 文件（内部方法）
    /// </summary>
    private static void CopyTableAndGenerateVersionInternal()
    {
        string sourceTablePath = Application.streamingAssetsPath + "/res/DataTable";
        string destTablePath = Application.persistentDataPath + "/res/DataTable";

        DeleteAllFile(destTablePath);
        if (File.Exists(ConstantVal.GetVersionStreamPath()))
            File.Delete(ConstantVal.GetVersionStreamPath());

        EncryptDirectoryFiles(new DirectoryInfo(sourceTablePath), new DirectoryInfo(destTablePath));
        CopyDirectory(new DirectoryInfo(sourceTablePath), new DirectoryInfo(destTablePath));
        GenerateTheVersion(Application.persistentDataPath + "/res");

        FileInfo fileInfo = new FileInfo(ConstantVal.GetVersionPersistentPath());
        fileInfo.CopyTo(ConstantVal.GetVersionStreamPath(), true);
        Debug.Log("[VersionEditor] 版本文件生成成功");
    }

    private static void GenerateTheVersion(string thePath)
    {
        var md5Generator = new System.Security.Cryptography.MD5CryptoServiceProvider();
        var DicFileMD5 = new Dictionary<string, string>();

        foreach (string filePath in Directory.GetFiles(thePath, "*.*", SearchOption.AllDirectories))
        {
            if (filePath.Contains(".meta") || filePath.Contains("TheVersion"))
                continue;

            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] hash = md5Generator.ComputeHash(file);
            string strMD5 = BitConverter.ToString(hash).Replace("-", "");
            file.Close();

            string key = filePath.Substring(filePath.IndexOf("res")).Replace('\\', '/');
            if (!DicFileMD5.ContainsKey(key))
                DicFileMD5.Add(key, strMD5);
        }

        string mm = ConstantVal.mm;
        string savePath = Path.Combine(Application.persistentDataPath, "TheVersion.txt");

        using (FileStream fs = new FileStream(savePath, FileMode.Create))
        using (StreamWriter sw = new StreamWriter(fs))
        {
            foreach (KeyValuePair<string, string> pair in DicFileMD5)
            {
                string temp = pair.Key + ":" + pair.Value;
                temp = CommonUtil.EncryptDES(temp, mm);
                sw.WriteLine(temp);
            }
        }
    }

    public static void DeleteAllFile(string path)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        if (!dir.Exists) return;

        foreach (FileInfo f in dir.GetFiles())
            f.Delete();

        foreach (DirectoryInfo d in dir.GetDirectories())
            DeleteAllFile(d.FullName);

        dir.Delete();
    }

    public static List<FileInfo> GetFile(string path, List<FileInfo> FileList)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        foreach (FileInfo f in dir.GetFiles())
            FileList.Add(f);

        foreach (DirectoryInfo d in dir.GetDirectories())
            GetFile(d.FullName, FileList);

        return FileList;
    }

    public static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
    {
        if (destination.FullName.Equals(source.FullName)) return;
        if (!destination.Exists) destination.Create();

        foreach (FileInfo file in source.GetFiles())
        {
            if (file.FullName.Contains(".meta")) continue;
            file.CopyTo(Path.Combine(destination.FullName, file.Name), true);
        }

        foreach (DirectoryInfo dir in source.GetDirectories())
        {
            string destinationDir = Path.Combine(destination.FullName, dir.Name);
            CopyDirectory(dir, new DirectoryInfo(destinationDir));
        }
    }

    public static void EncryptDirectoryFiles(DirectoryInfo source, DirectoryInfo destination)
    {
        if (destination.FullName.Equals(source.FullName)) return;
        if (!destination.Exists) destination.Create();

        foreach (FileInfo file in source.GetFiles())
        {
            if (file.FullName.Contains(".meta")) continue;
            Encrypt(file.FullName);
        }

        foreach (DirectoryInfo dir in source.GetDirectories())
        {
            string destinationDir = Path.Combine(destination.FullName, dir.Name);
            EncryptDirectoryFiles(dir, new DirectoryInfo(destinationDir));
        }
    }

    public static void Encrypt(string filePath)
    {
        string content = File.ReadAllText(filePath);
        string encryptedContent = CommonUtil.EncryptDES(content, ConstantVal.mm);
        File.WriteAllText(filePath, encryptedContent);
    }
}
