#!/bin/sh
#

TOOLS_FOLDER="Tools"
SPECFLOW="$TOOLS_FOLDER/specflow/specflow.exe"
NUNIT="$TOOLS_FOLDER/nunit/nunit-console.exe"

TEST_PROJECT_FOLDER="Beaver.Tests"
TEST_PROJECT_CSPROJ="$TEST_PROJECT_FOLDER/Beaver.Tests.csproj"
TEST_PROJECT_BIN="$TEST_PROJECT_FOLDER/bin/Debug/Beaver.Tests.dll"

TMP_TXT="$TEST_PROJECT_FOLDER/TestResult.txt"
TMP_XML="$TEST_PROJECT_FOLDER/TestResult.xml"
TMP_HTML="$TEST_PROJECT_FOLDER/TestResult.html"

echo "[1/5] Generating nunit tests"
mono $SPECFLOW generateall $TEST_PROJECT_CSPROJ

echo "[2/5] Building project"
cp $TEST_PROJECT_FOLDER/Libraries/nunit*.dll $TEST_PROJECT_FOLDER/bin/Debug/
xbuild $TEST_PROJECT_CSPROJ 2>&1 > /dev/null

echo "[3/5] Running tests"
mono $NUNIT --labels --out=$TMP_TXT --xml=$TMP_XML $TEST_PROJECT_BIN 2>&1 > /dev/null

echo "[4/5] Generating specflow report"
mono $SPECFLOW nunitexecutionreport $TEST_PROJECT_CSPROJ /xmlTestResult:$TMP_XML /out:$TMP_HTML

rm $TMP_TXT
rm $TMP_XML
rm $TEST_PROJECT_FOLDER/bin/Debug/nunit*.dll

echo "[5/5] Completed. You can view report here: file://localhost`pwd`/$TMP_HTML"