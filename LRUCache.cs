/*
LRU Cache - leetcode #146 - runtime beats 87.12% of c# submissions

Design and implement a data structure for Least Recently Used (LRU) cache. It should support the following operations: get and put.

get(key) - Get the value (will always be positive) of the key if the key exists in the cache, otherwise return -1.
put(key, value) - Set or insert the value if the key is not already present. When the cache reached its capacity, it should invalidate the least recently used item before inserting a new item.
*/

/*
Initial Plan
-Keep doubly linked list as a "queue" of sorts. Tail is the LRU node.
-Keep a Dictionary (key, node) for O(1) Get/Put runtime

Put
-Diciontary.Count == capacity
1) evict tail
1a) set tail to tail.prev, then tail.next = null
2) Dictionary.Remove(key)
-Dictionary.Count < capacity (do this either way)
1) create new node with value = value
2) add new node to queue as head
3) add to dictionary (key, node)
*special case if first item in cache: tail = current node


Get
1) Try to find key in dictionary
1a) not found - return -1
2) found, move node to front of queue
3) return node.value
*/

public class LRUCache {
    private Node head, tail;
    private Dictionary<int, Node> map;
    private readonly int capacity;

    public LRUCache(int capacity) {
        this.capacity = capacity;
        map = new Dictionary<int, Node>();
    }
    
    public int Get(int key) {
        if (!map.ContainsKey(key)) return -1;
        
        Node node = map[key];
        MoveToHead(node);
        
        return node.value;
    }

    public void Put(int key, int value) {
        if (capacity == 0) return; // or throw error
        
        if (map.ContainsKey(key)) {        
            UpdateCache(key, value);
        } else if (map.Count == capacity) {
            EvictTail();
            AddToCache(key, value);
        } else {
            AddToCache(key, value);
        }
    }
    
    private void MoveToHead(Node node) {
        if (node.prev == null) return;  // node is already head
            
        // remove node from list and close gap
        node.prev.next = node.next;
        
        if (tail == node) {
            tail = node.prev;
        } else {
            node.next.prev = node.prev;
        }
        
        // move node to head
        head.prev = node;
        node.prev = null;
        node.next = head;
        head = node;
    }
    
    private void EvictTail() {
        map.Remove(tail.key);
        tail = tail.prev;
        
        if (tail != null) {
            tail.next = null;
        }
    }
    
    private void AddToCache(int key, int value) {
        Node node = new Node(key, value);
        map.Add(key, node);
        
        if (head != null) {
            head.prev = node;
        }
        
        node.next = head;
        head = node;
        
        if (map.Count == 1) {
            tail = node;
        }
    }
    
    private void UpdateCache(int key, int value) {
        Node node = map[key];
        node.value = value;
        MoveToHead(node);
    }
}

public class Node {
    public Node prev;
    public Node next;
    public int key;
    public int value;
    
    public Node(int key, int value) {
        this.key = key;
        this.value = value;
    }
}

/*
Possible Test Cases
1) Cache of size 1, 0
2) double key (change value)
3) Put over capacity
4) Other combinations of put/get
*/