<!-- OMX:RUNTIME:START -->
<session_context>
**Session:** omx-1781620984724-c7ekns | 2026-06-16T14:43:05.445Z

**Native Subagent Routing:**
When spawning Codex native subagents, always set `agent_type` to an installed OMX role.
Use the most specific role (`architect`, `code-reviewer`, `critic`, `planner`, `debugger`, etc.); use `executor` only for generic implementation work.
Never omit `agent_type` for OMX work: untyped Task subagents appear as default subagents and lose role-specific prompts/routing.

**Repository Lookup Routing:** use normal Codex repository inspection tools/subagents as the default surface for simple read-only repository lookup and implementation context.
- Use `omx sparkshell -- <command>` only for explicit shell-native read-only evidence or `--tmux-pane` summaries; it does not replace raw evidence capture.

**Compaction Protocol:**
Before context compaction, preserve critical state:
1. Write progress checkpoint via `omx state write --input '<json>' --json`
2. Save key decisions via `omx notepad write-working --input '<json>' --json`
3. Before large Team work near compaction, reload `.omx/state/team/<team>/preflight-context.json`
4. If context is >80% full, proactively checkpoint state
</session_context>
<!-- OMX:RUNTIME:END -->
