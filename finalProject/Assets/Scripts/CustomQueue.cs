using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomQueue<T>
{
    T[] arr;
    int head = -1;
    int tail = -1;
    
    public int Length { get; private set; }

    public CustomQueue(int maxSize)
    {
        arr = new T[maxSize];
        Length = 0;
    }

    public bool Push(T e)
    {
        if (IsFull()) return false;
        head = (head + 1) % arr.Length;
        arr[head] = e;
        Length++;
        if (tail == -1) tail = head;
        return true;
    }

    public T Pop()
    {
        if (IsEmpty()) return default(T);
        T result = arr[tail];
        arr[tail] = default(T);
        Length--;
        tail = (tail + 1) % arr.Length;
        if (IsEmpty())
        {
            head = -1;
            tail = -1;
        }
        return result;
    }

    public T Peek()
    {
        if (IsEmpty()) return default(T);
        return arr[tail];
    }

    public bool IsEmpty()
    {
        return (Length == 0);
    }

    public bool IsFull()
    {
        return (Length == arr.Length);
    }
}
