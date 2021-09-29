#!/usr/bin/env groovy
@Library('pipeline-library')_

def repoName = "pdfOcr"
def dependencyRegex = "itextcore"
def solutionFile = "i7n-ocr.sln"
def csprojFramework = "net461"

automaticDotnetBuild(repoName, dependencyRegex, solutionFile, csprojFramework)
