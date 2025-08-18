#!/usr/bin/env groovy
@Library('pipeline-library')_

def repoName = "pdfOcr"
def dependencyRegex = "itextcore"
def solutionFile = "i7n-ocr.sln"
def frameworksToTest = "net461"
def frameworksToTestForMainBranches = "net461;netcoreapp2.0"


automaticDotnetBuild(repoName, dependencyRegex, solutionFile, frameworksToTest, frameworksToTestForMainBranches)
