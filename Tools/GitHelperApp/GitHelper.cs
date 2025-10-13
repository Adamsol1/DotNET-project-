using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DOTNET_PROJECT.Tools;

class GitHelper 
{

	// main runs all the commands
	public static int Run(string[] args)
	{
		Console.WriteLine("GitHelper for dummies,");
		Console.WriteLine("This file is meant to help you with git pull/push if you cant do it in Desktop or commandline:");

		var repoPath = GetRepoRoot() ?? Directory.GetCurrentDirectory();
		Console.WriteLine($"Your repository path: {repoPath}");

		// find your current Branch
		var currentBranch = TryGetBranch(repoPath);

		if (currentBranch == null) {
			Console.WriteLine("No branch found");
			return 1;
		}

		// if your current branch is not Div switch to it.
		if (!currentBranch.Equals("Dev", StringComparison.OrdinalIgnoreCase)) {
			Console.WriteLine("Your current branch is not Dev,");
			
			Console.WriteLine($"> Bytter fra {currentBranch} til Dev");

			if (RunStep("checkout Dev", repoPath) != 0) return 1;
		}

		var msg = GetInput("Write your commit message: ", "update");

		if (RunStep("add -A", repoPath) != 0) return 1;

		var commitCode = RunGit($"commit -m {Quote(msg)}", repoPath, out var commitOut, out var commitErr);

		Console.WriteLine(commitOut);
		if (!string.IsNullOrWhiteSpace(commitErr)) Console.Error.WriteLine(commitErr);

		// if nothing to commit then, continue
		if (commitCode != 0 && !(commitErr + commitOut).Contains("nothing to commit", StringComparison.OrdinalIgnoreCase))
		{
			Console.Error.WriteLine("Commit feilet.");
			return commitCode;
		}

		// fetch + pull --rebase
				if (RunStep("fetch origin", repoPath) != 0) return 1;

		Console.WriteLine("> Pull (rebase) fra origin/Dev før push ...");
		var pullCode = RunGit("pull --rebase origin Dev", repoPath, out var pullOut, out var pullErr);
		Console.WriteLine(pullOut);
		if (!string.IsNullOrWhiteSpace(pullErr)) Console.Error.WriteLine(pullErr);

		if (pullCode != 0)
		{
			Console.Error.WriteLine("Pull --rebase ga konflikter eller feilet. Løs konfliktene manuelt:");
			Console.Error.WriteLine("  1) Løs konflikter i filer");
			Console.Error.WriteLine("  2) git add <filer>");
			Console.Error.WriteLine("  3) git rebase --continue");
			Console.Error.WriteLine("  4) Kjør verktøyet på nytt for push");
			return pullCode;
		}

		// Sjekk om upstream er satt
		var hasUpstream = RunGit("rev-parse --abbrev-ref --symbolic-full-name @{u}", repoPath, out var _, out var _) == 0;

		// push
		var pushArgs = hasUpstream ? "push" : "push -u origin Dev";
		if (RunStep(pushArgs, repoPath) != 0) return 1;

		Console.WriteLine("Ferdig: endringer pushet til Dev.");
		return 0;
	
	}

	static string GetInput(string prompt, string @default)
	{
		Console.Write(prompt);
		var input = Console.ReadLine();
		return string.IsNullOrWhiteSpace(input) ? @default : input.Trim();
	}

	static string? TryGetBranch(string repoPath)
	{
		return RunGit("rev-parse --abbrev-ref HEAD", repoPath, out var o, out var _) == 0 ? o.Trim() : null;
	}

	static int RunStep(string args, string workingDir)
	{
		Console.WriteLine($"> git {args}");
		var code = RunGit(args, workingDir, out var stdout, out var stderr);
		if (!string.IsNullOrWhiteSpace(stdout)) Console.WriteLine(stdout);
		if (!string.IsNullOrWhiteSpace(stderr)) Console.Error.WriteLine(stderr);
		return code;
	}

	static int RunGit(string args, string workingDir, out string stdout, out string stderr)
	{
		var gitPath = GetGitExecutablePath();
		var psi = new ProcessStartInfo
		{
			FileName = gitPath,
			Arguments = args,
			WorkingDirectory = workingDir,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true,
			StandardOutputEncoding = Encoding.UTF8,
			StandardErrorEncoding = Encoding.UTF8
		};

		using var p = new Process { StartInfo = psi };
		var sbOut = new StringBuilder();
		var sbErr = new StringBuilder();
		p.OutputDataReceived += (_, e) => { if (e.Data != null) sbOut.AppendLine(e.Data); };
		p.ErrorDataReceived += (_, e) => { if (e.Data != null) sbErr.AppendLine(e.Data); };

		p.Start();
		p.BeginOutputReadLine();
		p.BeginErrorReadLine();
		p.WaitForExit();

		stdout = sbOut.ToString();
		stderr = sbErr.ToString();
		return p.ExitCode;
	}

	static string GetGitExecutablePath()
	{
		// On macOS, git is typically at /usr/bin/git; fall back to PATH
		var macGit = "/usr/bin/git";
		if (OperatingSystem.IsMacOS() && File.Exists(macGit)) return macGit;
		return "git";
	}

	static string? GetRepoRoot()
	{
		try
		{
			var code = RunGit("rev-parse --show-toplevel", Directory.GetCurrentDirectory(), out var outStr, out var _);
			if (code == 0)
			{
				var root = outStr.Trim();
				if (!string.IsNullOrWhiteSpace(root) && Directory.Exists(root)) return root;
			}
		}
		catch { /* ignore */ }
		return null;
	}

	static string Quote(string s)
	{
		if (string.IsNullOrEmpty(s)) return "\"\"";
		if (s.Contains('"')) s = s.Replace("\"", "\\\"");
		return $"\"{s}\"";
	}
}

