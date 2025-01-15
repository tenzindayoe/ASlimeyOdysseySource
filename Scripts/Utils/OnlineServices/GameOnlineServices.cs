using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class GameOnlineServices
{
    private readonly string baseURL;

    public GameOnlineServices(string baseURL)
    {
        this.baseURL = baseURL;
    }

    public async Task<List<LakeData>> GetMajorLakes()
    {
        string url = $"{baseURL}/getMajorLakes";
        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone) await Task.Yield();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error fetching major lakes: {request.error}");
                    return null;
                }

                Debug.Log("Fetched Major Lakes Data");
                return JsonUtility.FromJson<LakeListWrapper>($"{{\"lakes\":{request.downloadHandler.text}}}").lakes;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception in GetMajorLakes: {e.Message}");
            return null;
        }
    }

    public async Task<List<LakeData>> GetSideLakes()
    {
        string url = $"{baseURL}/getSideLakes";
        try
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone) await Task.Yield();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"Error fetching side lakes: {request.error}");
                    return null;
                }

                Debug.Log("Fetched Side Lakes Data");
                return JsonUtility.FromJson<LakeListWrapper>(request.downloadHandler.text).lakes;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception in GetSideLakes: {e.Message}");
            return null;
        }
    }
}

[System.Serializable]
public class LakeData
{
    public string name;
    public double latitude;
    public double longitude;
    public double area_km2;
}

[System.Serializable]
public class LakeListWrapper
{
    public List<LakeData> lakes;
}
