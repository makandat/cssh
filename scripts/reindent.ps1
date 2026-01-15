# Reindent .cs files to 2-space using brace level heuristic
Get-ChildItem -Recurse -Filter *.cs | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } | ForEach-Object {
  $path = $_.FullName
  $lines = Get-Content -LiteralPath $path
  $out = New-Object System.Collections.Generic.List[System.String]
  $indent = 0
  foreach ($line in $lines) {
    $trim = $line.Trim()
    if ($trim -eq '') {
      $out.Add('')
      continue
    }

    $open = ($trim.ToCharArray() | Where-Object { $_ -eq '{' }).Count
    $close = ($trim.ToCharArray() | Where-Object { $_ -eq '}' }).Count

    if ($trim.StartsWith('}')) {
      $indent = [Math]::Max(0, $indent - $close)
    }

    $out.Add((' ' * ($indent * 2)) + $trim)

    if ($trim.StartsWith('}')) {
      $indent = $indent + $open
    }
    else {
      $indent = $indent + ($open - $close)
    }
  }

  Set-Content -LiteralPath $path -Value $out -Encoding UTF8
  Write-Host "Reindented: $path"
}
